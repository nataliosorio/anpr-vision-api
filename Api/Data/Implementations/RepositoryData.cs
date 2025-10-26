using AutoMapper;
using AutoMapper.QueryableExtensions;
using Data.Interfaces;
using Entity.Annotations;
using Entity.Contexts;
using Entity.Contexts.parking;
using Entity.Dtos;
using Entity.Models;
using Entity.Models.Audit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Utilities.Audit.Services;
using Utilities.Exceptions;
using Utilities.Helpers;
using Utilities.Interfaces;
using static Dapper.SqlMapper;

namespace Data.Implementations
{
    public class RepositoryData<T> : ARepositoryData<T> where T : BaseModel
    {
        protected readonly ApplicationDbContext _context;
        protected readonly IConfiguration _configuration;
        private readonly IAuditService _auditService;
        private readonly ICurrentUserService _currentUserService;
        protected readonly IMapper _mapper;
        protected readonly IParkingContext _parkingContext; // 👈 nuevo

        public RepositoryData(
            ApplicationDbContext context,
            IConfiguration configuration,
            IAuditService auditService,
            ICurrentUserService currentUserService,
            IMapper mapper,
            IParkingContext parkingContext) // 👈 agregado
        {
            _context = context;
            _configuration = configuration;
            _auditService = auditService;
            _currentUserService = currentUserService;
            _mapper = mapper;
            _parkingContext = parkingContext;
        }

        // ============================================================
        // 🔍 Helper para saber si la entidad tiene ParkingId
        // ============================================================
        private static bool HasParkingIdProperty() =>
            typeof(T).GetProperty("ParkingId") != null;

        public IQueryable<T> ApplyParkingFilter(IQueryable<T> query)
        {
            if (HasParkingIdProperty() && _parkingContext.ParkingId.HasValue)
            {
                var parkingId = _parkingContext.ParkingId.Value;
                query = query.Where(e => EF.Property<int>(e, "ParkingId") == parkingId);
            }
            return query;
        }

        // ============================================================
        // 🔹 ExistsAsync
        // ============================================================
        public override async Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate)
        {
            var query = _context.Set<T>().AsQueryable();
            query = ApplyParkingFilter(query);
            return await query.AnyAsync(predicate);
        }

        protected async Task AuditAsync(string action, int entityId = 0, string changes = null) { var entry = new AuditLog { Action = action, EntityName = typeof(T).Name, EntityId = entityId, Timestamp = DateTime.UtcNow, UserName = _currentUserService.UserName ?? "SYSTEM", Changes = changes ?? "Sin detalles" }; await _auditService.SaveAuditAsync(entry); }

        // ============================================================
        // 🔹 GetAll
        // ============================================================
        public override async Task<IEnumerable<T>> GetAll(IDictionary<string, string?>? filters = null)
        {
            try
            {
                var query = _context.Set<T>().AsQueryable().Where(e => e.IsDeleted == false); 
                query = ApplyParkingFilter(query);

                if (filters != null && filters.Any())
                    query = query.ApplyFilters(filters);

                return await query.ToListAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        // ============================================================
        // 🔹 GetById
        // ============================================================
        public override async Task<T> GetById(int id)
        {
            var query = _context.Set<T>().AsNoTracking().Where(e => e.IsDeleted == false);
            query = ApplyParkingFilter(query);
            return await query.FirstOrDefaultAsync(i => i.Id == id);
        }

        // ============================================================
        // 🔹 Save
        // ============================================================
        public override async Task<T> Save(T entity)
        {
            try
            {
                if (HasParkingIdProperty() && _parkingContext.ParkingId.HasValue)
                {
                    var property = typeof(T).GetProperty("ParkingId");
                    var currentValue = property.GetValue(entity);
                    if (currentValue == null || (int)currentValue == 0)
                        property.SetValue(entity, _parkingContext.ParkingId.Value);
                }

                _context.Set<T>().Add(entity);
                await _context.SaveChangesAsync();
                return entity;
            }
            catch (DbException ex)
            {
                Console.WriteLine("Database error: " + ex.Message);
                throw;
            }
        }

        // ============================================================
        // 🔹 Update
        // ============================================================
        public override async Task Update(T entity)
        {
            try
            {
                if (HasParkingIdProperty() && _parkingContext.ParkingId.HasValue)
                {
                    var property = typeof(T).GetProperty("ParkingId");
                    var currentValue = property.GetValue(entity);
                    if (currentValue == null || (int)currentValue == 0)
                        property.SetValue(entity, _parkingContext.ParkingId.Value);
                }

                _context.Entry(entity).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                _context.Entry(entity).State = EntityState.Detached;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error actualizando entidad {typeof(T).Name}: {ex.Message}");
                throw;
            }
        }

        // ============================================================
        // 🔹 PermanentDelete
        // ============================================================
        public override async Task<bool> PermanentDelete(int id)
        {
            if (id <= 0)
                throw new DataException("El ID proporcionado no es válido. Debe ser mayor a cero.");

            var query = _context.Set<T>().AsQueryable();
            query = ApplyParkingFilter(query);
            var entity = await query.FirstOrDefaultAsync(d => d.Id == id);

            if (entity == null)
                throw new DataException($"No se encontró un registro con el ID {id}.");

            try
            {
                _context.Remove(entity);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new DataException("Error al eliminar el registro.", ex);
            }
        }

        // ============================================================
        // 🔹 Delete (soft delete)
        // ============================================================
        public override async Task<int> Delete(int id)
        {
            var query = _context.Set<T>().AsQueryable();
            query = ApplyParkingFilter(query);

            var entity = await query.FirstOrDefaultAsync(d => d.Id == id);
            if (entity == null)
                throw new DataException($"No se encontró un registro con el ID {id}.");

            entity.IsDeleted = true;
            _context.Entry(entity).State = EntityState.Modified;
            return await _context.SaveChangesAsync();
        }

        // ============================================================
        // 🔹 GetAllDynamicAsync
        // ============================================================
        public override async Task<List<ExpandoObject>> GetAllDynamicAsync()
        {
            var entityType = typeof(T);
            var query = _context.Set<T>().AsQueryable();
            query = ApplyParkingFilter(query);

            var foreignKeyProps = entityType
                .GetProperties()
                .Where(p => Attribute.IsDefined(p, typeof(ForeignIncludeAttribute)))
                .ToList();

            foreach (var prop in foreignKeyProps)
                query = query.Include(prop.Name);

            var resultList = await query.ToListAsync();
            var dynamicList = new List<ExpandoObject>();

            foreach (var entity in resultList)
            {
                dynamic dyn = new ExpandoObject();
                var dict = (IDictionary<string, object?>)dyn;
                dict["Id"] = entityType.GetProperty("Id")?.GetValue(entity);

                foreach (var prop in foreignKeyProps)
                {
                    var attr = prop.GetCustomAttribute<ForeignIncludeAttribute>()!;
                    var foreignValue = prop.GetValue(entity);
                    if (foreignValue == null) continue;

                    if (attr.SelectPaths == null || attr.SelectPaths.Length == 0)
                    {
                        dict[prop.Name] = foreignValue;
                    }
                    else
                    {
                        foreach (var path in attr.SelectPaths)
                        {
                            var value = ReflectionHelper.GetNestedPropertyValue(foreignValue, path);
                            var key = ReflectionHelper.PascalJoin(prop.Name, path);
                            dict[key] = value;
                        }
                    }
                }

                dynamicList.Add(dyn);
            }

            return dynamicList;
        }

        // ============================================================
        // 🔹 GetAllPaginatedAsync
        // ============================================================
        public override async Task<PagedResult<TDto>> GetAllPaginatedAsync<TDto>(
            QueryParameters query,
            Expression<Func<T, bool>>? filter = null,
            Func<IQueryable<T>, IQueryable<T>>? include = null,
            CancellationToken cancellationToken = default)
        {
            var dbQuery = _context.Set<T>().AsNoTracking();
            dbQuery = ApplyParkingFilter(dbQuery);

            if (include is not null)
                dbQuery = include(dbQuery);

            if (filter is not null)
                dbQuery = dbQuery.Where(filter);

            var totalCount = await dbQuery.CountAsync(cancellationToken);

            var items = await dbQuery
                .ProjectTo<TDto>(_mapper.ConfigurationProvider)
                .Skip((query.Page - 1) * query.PageSize)
                .Take(query.PageSize)
                .ToListAsync(cancellationToken);

            return new PagedResult<TDto>
            {
                Items = items,
                TotalCount = totalCount,
                Page = query.Page,
                PageSize = query.PageSize
            };
        }

        // ============================================================
        // 🔹 ExistsAsynca
        // ============================================================
        public override async Task<bool> ExistsAsynca(string field, string value, int? currentId)
        {
            var entityType = typeof(T);
            var property = entityType.GetProperty(field);
            if (property == null)
                throw new ArgumentException($"El campo '{field}' no existe en '{entityType.Name}'.");

            var query = _context.Set<T>().AsQueryable();
            query = ApplyParkingFilter(query);

            var parameter = Expression.Parameter(entityType, "x");
            var propertyAccess = Expression.Property(parameter, property);
            var constant = Expression.Constant(Convert.ChangeType(value, property.PropertyType));
            var equalExpression = Expression.Equal(propertyAccess, constant);

            if (currentId.HasValue)
            {
                var idProperty = Expression.Property(parameter, "Id");
                var notCurrentId = Expression.NotEqual(idProperty, Expression.Constant(currentId.Value));
                equalExpression = Expression.AndAlso(equalExpression, notCurrentId);
            }

            var lambda = Expression.Lambda<Func<T, bool>>(equalExpression, parameter);
            return await query.AnyAsync(lambda);
        }

      

    }

}
