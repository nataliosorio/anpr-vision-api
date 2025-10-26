using AutoMapper;
using Data.Interfaces.Parameter;
using Entity.Contexts;
using Entity.Contexts.parking;
using Entity.Dtos.Parameter;
using Entity.Models.Parameter;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Utilities.Audit.Services;
using Utilities.Interfaces;

namespace Data.Implementations.Parameter
{
    public class ZonesData : RepositoryData<Zones>, IZonesData
    {
        private readonly ILogger<ZonesData> _logger;

        public ZonesData(
            ApplicationDbContext context,
            IConfiguration configuration,
            IAuditService auditService,
            ICurrentUserService currentUserService,
            IMapper mapper,
            ILogger<ZonesData> logger,
            IParkingContext parkingContext)
            : base(context, configuration, auditService, currentUserService, mapper, parkingContext)
        {
            _logger = logger;
        }

        // ============================================================
        // 🔹 GetAllJoinAsync — aplica filtro automático de parking
        // ============================================================
        public async Task<IEnumerable<ZonesDto>> GetAllJoinAsync()
        {
            var query = _context.Zones.AsNoTracking();

            // ✅ Filtra automáticamente según el token
            if (_parkingContext.ParkingId.HasValue)
            {
                var pid = _parkingContext.ParkingId.Value;
                query = query.Where(z => z.ParkingId == pid);
            }

            return await query
                .Select(p => new ZonesDto
                {
                    Id = p.Id,
                    Asset = p.Asset,
                    IsDeleted = p.IsDeleted,
                    Name = p.Name,
                    ParkingId = p.ParkingId,
                    Parking = p.Parking != null ? p.Parking.Name : null
                })
                .ToListAsync();
        }

        // ============================================================
        // 🔹 GetAllByParkingId 
        // ============================================================
        //public async Task<IEnumerable<Zones>> GetAllByParkingId(int parkingId)
        //{
        //    return await _context.Zones
        //        .AsNoTracking()
        //        .Where(s => s.ParkingId == parkingId && s.IsDeleted == false)
        //        .ToListAsync();
        //}
    }
}
