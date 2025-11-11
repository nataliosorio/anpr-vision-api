using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Data.Interfaces.Parameter;
using Entity.Contexts;
using Entity.Contexts.parking;
using Entity.Dtos.Parameter;
using Entity.Models.Parameter;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Utilities.Audit.Services;
using Utilities.Interfaces;

namespace Data.Implementations.Parameter
{
    public class CameraData : RepositoryData<Camera>, ICamaraData
    {
        private readonly ILogger<CameraData> _logger;

        public CameraData(ApplicationDbContext context, IConfiguration configuration, IAuditService auditService, ICurrentUserService currentUserService, IMapper mapper, ILogger<CameraData> logger, IParkingContext parkingContext)
            : base(context, configuration, auditService, currentUserService, mapper, parkingContext)
        {
            _logger = logger;
            



        }

        //public async Task<IEnumerable<CameraDto>> GetAllJoinAsync()
        //{
        //    return await _context.Cameras
        //        .AsNoTracking()
        //        .Select(p => new CameraDto
        //        {
        //            // --- BaseDto ---
        //            Id = p.Id,                      // int? en BaseDto
        //            Asset = p.Asset,                 // bool? en BaseDto
        //            IsDeleted = p.IsDeleted,         // bool en BaseDto

        //            // --- GenericDto ---

        //            Name = p.Name,
        //            Resolution = p.Resolution,
        //            Url = p.Url,

        //            // --- ZonesDto ---
        //            ParkingId = p.ParkingId,
        //            Parking = p.Parking != null
        //                ? p.Parking.Name
        //                : null
        //        })
        //        .ToListAsync();
        //}
        public async Task<IEnumerable<CameraDto>> GetAllJoinAsync()
        {
            var query = _context.Cameras.AsNoTracking().Where(c => c.IsDeleted == false);

            // ✅ Filtra automáticamente según el token
            if (_parkingContext.ParkingId.HasValue)
            {
                var pid = _parkingContext.ParkingId.Value;
                query = query.Where(c => c.ParkingId == pid);
            }

            return await query
                .Select(p => new CameraDto
                {
                    Id = p.Id,
                    Asset = p.Asset,
                    IsDeleted = p.IsDeleted,
                    Name = p.Name,
                    Resolution = p.Resolution,
                    Url = p.Url,
                    ParkingId = p.ParkingId,
                    Parking = p.Parking != null ? p.Parking.Name : null
                })
                .ToListAsync();
        }

        public async Task<bool> ExistsDuplicateAsync(CameraDto dto)
        {
            return await _context.Cameras
                .AsNoTracking()
                .AnyAsync(x =>
                    x.Resolution.Trim().ToLower() == dto.Resolution.Trim().ToLower() &&
                    x.Url.Trim().ToLower() == dto.Url.Trim().ToLower() &&
                    x.ParkingId == dto.ParkingId &&
                    x.Id != dto.Id &&
                    x.Asset == true
                );
        }

        public async Task<IEnumerable<CameraDto>> GetByParkingAsync(int parkingId)
        {
            return await _context.Cameras
                .AsNoTracking()
                .Where(c => c.ParkingId == parkingId && (c.IsDeleted == null || c.IsDeleted == false))
                .Select(p => new CameraDto
                {
                    Id = p.Id,
                    Asset = p.Asset,
                    IsDeleted = p.IsDeleted,
                    Name = p.Name,
                    Resolution = p.Resolution,
                    Url = p.Url,
                    ParkingId = p.ParkingId,
                    Parking = p.Parking != null ? p.Parking.Name : null
                })
                .ToListAsync();
        }




    }
}
