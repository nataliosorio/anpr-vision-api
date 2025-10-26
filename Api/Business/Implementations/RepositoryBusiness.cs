using AutoMapper;
using Business.Interfaces;
using Data.Interfaces;
using Entity.Dtos;
using Entity.Models;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Utilities.Exceptions;
using static Dapper.SqlMapper;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Microsoft.EntityFrameworkCore; 

namespace Business.Implementations
{
    public class RepositoryBusiness<T, D> : ARepositoryBusiness<T, D> where T : BaseModel where D : BaseDto
    {
        private readonly IRepositoryData<T> _data;
        private readonly IMapper _mapper;

        public RepositoryBusiness(IRepositoryData<T> data, IMapper mapper)
        {
            _data = data;
            _mapper = mapper;
        }

        public override async Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate)
        {
            try
            {
                return await _data.ExistsAsync(predicate);
            }
            catch (Exception ex)
            {
                throw new BusinessException("Error al validar existencia de la entidad.", ex);
            }
        }


        public override async Task<bool> PermanentDelete(int id)
        {
            if (id <= 0)
                throw new ValidationException(nameof(id), "El ID debe ser mayor que cero.");

            var success = await _data.PermanentDelete(id);
            if (!success)
                throw new EntityNotFoundException(typeof(T).Name, id);

            return true;
        }
        public override async Task<int> Delete(int id)
        {
            if (id <= 0)
                throw new ValidationException(nameof(id), "El ID debe ser mayor que cero.");
            return await _data.Delete(id);
        }

        public override async Task<List<D>> GetAll(IDictionary<string, string?>? filters = null)
        {
            try
            {
                var entities = await _data.GetAll(filters);
                var dtos = _mapper.Map<List<D>>(entities);
                if (dtos.Count <= 0)
                    throw new InvalidOperationException("No se encontraron registros.");
                return dtos;
            }
            catch (InvalidOperationException invEx)
            {
                throw new InvalidOperationException($"Error: {invEx.Message}", invEx);
            }
            catch (Exception ex)
            {
                throw new ExternalServiceException("Repositorio", "Error al obtener todos los registros", ex);
            }
        }

        public override async Task<D> GetById(int id)
        {
            if (id <= 0)
                throw new ValidationException(nameof(id), "El ID debe ser mayor que cero.");
            T entity = await _data.GetById(id);
            if (entity == null)
                throw new EntityNotFoundException(typeof(T).Name, id);
            D dto = _mapper.Map<D>(entity);
            return dto;
        }


        public override async Task<D> Save(D dto)
        {
            try
            {
                dto.Asset = true;
                BaseModel entity = _mapper.Map<T>(dto);
                entity = await _data.Save((T)entity);
                return _mapper.Map<D>(entity);
            }
            catch (Exception ex)
            {
                throw new BusinessException("Error al crear la entidad.", ex);
            }
        }

        public override async Task Update(D dto)
        {
            try
            {
                BaseModel entity = _mapper.Map<T>(dto);
                await _data.Update((T)entity);
            }
            catch (Exception ex)
            {
                throw new BusinessException("Error al actualizar la entidad.", ex);
            }
        }

        /// <summary>
        /// Obtiene una lista dinámica de entidades con relaciones incluidas automáticamente.
        /// </summary>
        /// <returns>Lista de objetos dinámicos con propiedades en PascalCase.</returns>
        public override async Task<List<ExpandoObject>> GetAllDynamicAsync()
        {
            return await _data.GetAllDynamicAsync();
        }

        public override async Task<PagedResult<D>> GetAllPaginatedAsync(QueryParameters query, Expression<Func<T, bool>>? filter = null, Func<IQueryable<T>, IQueryable<T>>? include = null, CancellationToken cancellationToken = default)
        {
            try
            {
                var pagedResultDto = await _data.GetAllPaginatedAsync<D>(query, filter, include, cancellationToken);
                return pagedResultDto;
            }
            catch (Exception ex)
            {
                throw new BusinessException("Error al obtener listado paginado", ex);
            }
        }

        public override async Task<bool> ExistsAsynca(string field, string value, int? currentId)
        {
           
            var param = Expression.Parameter(typeof(T), "e");

  
            var efProperty = Expression.Call(
                typeof(EF),
                nameof(EF.Property),
                new[] { typeof(string) },
                param,
                Expression.Constant(field)
            );

    
            var equals = Expression.Equal(
                efProperty,
                Expression.Constant(value, typeof(string))
            );

            Expression body = equals;

            if (currentId.HasValue)
            {
                var idProp = Expression.Property(param, nameof(BaseModel.Id));
                var notSameId = Expression.NotEqual(idProp, Expression.Constant(currentId.Value));
                body = Expression.AndAlso(body, notSameId);
            }

            var predicate = Expression.Lambda<Func<T, bool>>(body, param);

            return await _data.ExistsAsync(predicate);
        }

    }
}
