using AutoMapper;
using Data.Interfaces.Security;
using Entity.Contexts;
using Entity.Contexts.parking;
using Entity.Models.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities.Audit.Services;
using Utilities.Interfaces;

namespace Data.Implementations.Security
{
    public class RolData : RepositoryData<Rol>, IRolData
    {
        public RolData(ApplicationDbContext context, IConfiguration configuration,IAuditService auditService, ICurrentUserService currentUserService, IMapper mapper, IParkingContext parkingContext)
            : base(context, configuration,  auditService, currentUserService, mapper, parkingContext)
        {

        }

        public async Task<IEnumerable<Rol>> GetAllByParkingAsync()
        {
            var parkingId = _parkingContext.ParkingId; // 👈 del contexto actual

            var roles = await (
                from r in _context.Rol.AsNoTracking()
                join rpu in _context.RolParkingUsers on r.Id equals rpu.RolId
                where rpu.ParkingId == parkingId && (r.IsDeleted == false || r.IsDeleted == null)
                select new Rol
                {
                    Id = r.Id,
                    Name = r.Name,
                    Description = r.Description,
                    Asset = r.Asset,
                    IsDeleted = r.IsDeleted
                }
            ).Distinct().ToListAsync();

            return roles;
        }

        public async Task<Rol?> GetByNameAsync(string name)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(name)) return null;
                var normalized = name.Trim().ToUpperInvariant();

                return await _context.Set<Rol>()
                    .AsNoTracking()
                    .FirstOrDefaultAsync(r =>
                        r.Name != null &&
                        r.Name.ToUpper() == normalized &&
                        !(r.IsDeleted ?? false)
                    );
            }
            catch (Exception ex)
            {
                throw new DataException("Error al obtener el rol por nombre", ex);
            }
        }

        // Si RepositoryData tiene 'virtual Task Update(T entity)' puedes sobreescribir:
        public override async Task Update(Rol entity)
        {
            var dbEntity = await _context.Set<Rol>().FindAsync(entity.Id);
            if (dbEntity == null)
                throw new InvalidOperationException($"No existe el rol con Id {entity.Id}.");

            if (!ReferenceEquals(dbEntity, entity))
            {
                _context.Entry(dbEntity).CurrentValues.SetValues(entity);
            }

            await _context.SaveChangesAsync();
        }
      
    }
}
