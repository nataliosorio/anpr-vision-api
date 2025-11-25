using AutoMapper;
using Data.Interfaces.Security;
using Entity.Contexts;
using Entity.Contexts.parking;
using Entity.Models.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities.Audit.Services;
using Utilities.Exceptions;
using Utilities.Helpers;
using Utilities.Interfaces;

namespace Data.Implementations.Security
{
    public class RolParkingUserData : RepositoryData<RolParkingUser>, IRolParkingUserData
    {
        private readonly ILogger<RolParkingUserData> _logger;

        public RolParkingUserData(ApplicationDbContext context, IConfiguration configuration,  ILogger<RolParkingUserData> logger, IAuditService auditService, ICurrentUserService currentUserService, IMapper mapper, IParkingContext parkingContext)
            : base(context, configuration, auditService, currentUserService, mapper, parkingContext)
        {
            _logger = logger;
        }

        public async Task<IEnumerable<RolParkingUser>> GetAllJoinAsync()
        {
            //await AuditAsync("GetAllJoinAsync");

            return await _context.RolParkingUsers
                .Include(x => x.User)
                .Include(x => x.Rol)
                .Include(x => x.Parking)
                .ToListAsync();
        }


        public override async Task<IEnumerable<RolParkingUser>> GetAll(IDictionary<string, string?>? filters = null)
        {
            var query = _context.RolParkingUsers
                .Include(ru => ru.User)
                .Include(ru => ru.Rol)
                .Include(ru => ru.Parking)
                .AsQueryable();

            // si quieres, también puedes aplicar filtros genéricos aquí
            if (filters != null && filters.Any())
                query = query.ApplyFilters(filters);

            var entities = await query.ToListAsync();

            //await AuditAsync("GetAll");

            return entities;
        }

        public override async Task<RolParkingUser> GetById(int id)
        {
            await AuditAsync("GetById", id);
            return await _context.RolParkingUsers
                .Include(ru => ru.User)
                .Include(ru => ru.Rol)
                .Include(ru => ru.Parking)
                .FirstOrDefaultAsync(ru => ru.Id == id);
        }

        public async Task<bool> ExistsAsync(int userId, int roleId)
        {
            try
            {
                return await _context.Set<RolParkingUser>()
                        .AnyAsync(ur => ur.UserId == userId && ur.RolId == roleId );
            }
            catch (Exception ex)
            {

                throw  new DataException("Error al obtener el rol por nombre", ex);
            }
        }

        public async Task<IEnumerable<RolParkingUser>> GetByUserIdAsync(int userId)
        {
            try
            {
                return await _context.RolParkingUsers
                    .Include(ru => ru.User)
                    .Include(ru => ru.Rol)
                    .Include(ru => ru.Parking)
                    .Where(ru => ru.UserId == userId
                              && (ru.IsDeleted == null || ru.IsDeleted == false))
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener roles por usuario {UserId}", userId);
                throw new DataException("Error al obtener los roles del usuario.", ex);
            }
        }

    }
}
