using AutoMapper;
using Data.Interfaces.Parameter;
using Entity.Contexts;
using Entity.Contexts.parking;
using Entity.Dtos.Parameter;
using Entity.Models.Parameter;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities.Audit.Services;
using Utilities.Interfaces;

namespace Data.Implementations.Parameter
{
    public class SectorsData : RepositoryData<Sectors>, ISectorsData
    {
        public SectorsData(ApplicationDbContext context, IConfiguration configuration, IAuditService auditService, ICurrentUserService currentUserService, IMapper mapper, IParkingContext parkingContext)
            : base(context, configuration, auditService, currentUserService, mapper, parkingContext)
        {

        }

        //public async Task<IEnumerable<SectorsDto>> GetAllJoinAsync()
        //{
        //    return await _context.Sectors
        //        .AsNoTracking()
        //        .Select(p => new SectorsDto
        //        {
        //            // --- BaseDto ---
        //            Id = p.Id,                      // int? en BaseDto
        //            Asset = p.Asset,                 // bool? en BaseDto
        //            IsDeleted = p.IsDeleted,         // bool en BaseDto

        //            // --- GenericDto ---
        //            Name = p.Name,                   // string en GenericDto

        //            // --- SectorsDto ---
        //            Capacity = p.Capacity,
        //            ZonesId = p.ZonesId,
        //            Zones = p.Zones != null
        //                ? p.Zones.Name
        //                : null,
        //            TypeVehicleId = p.TypeVehicleId,
        //            TypeVehicle = p.TypeVehicle != null
        //                ? p.TypeVehicle.Name
        //                : null
        //        })
        //        .ToListAsync();
        //}

        public async Task<IEnumerable<SectorsDto>> GetAllJoinAsync()
        {
            var parkingId = _parkingContext.ParkingId; 

            return await _context.Sectors
                .AsNoTracking()
                .Include(s => s.Zones)
                .Include(s => s.TypeVehicle)
                .Where(s => s.Zones.ParkingId == parkingId && (s.IsDeleted == false || s.IsDeleted == null)) 
                .Select(p => new SectorsDto
                {
                    // --- BaseDto ---
                    Id = p.Id,
                    Asset = p.Asset,
                    IsDeleted = p.IsDeleted,

                    // --- GenericDto ---
                    Name = p.Name,

                    // --- SectorsDto ---
                    Capacity = p.Capacity,
                    ZonesId = p.ZonesId,
                    Zones = p.Zones != null ? p.Zones.Name : null,
                    TypeVehicleId = p.TypeVehicleId,
                    TypeVehicle = p.TypeVehicle != null ? p.TypeVehicle.Name : null
                })
                .ToListAsync();
        }


        public async Task<IEnumerable<Sectors>> GetAllByZoneId(int zoneId)
        {
            return await _context.Sectors
                .AsNoTracking()
                .Where(s => s.ZonesId == zoneId && s.IsDeleted == false)
                .ToListAsync();
        }

        //public async Task<List<Sectors>> GetSectorsByVehicleTypeAsync(int vehicleTypeId, int parkingId)
        //{
        //    var sectors = await _context.Sectors
        //        .Include(s => s.Slots)
        //        .Include(s => s.Zones) // importante para poder filtrar por ParkingId
        //        .Where(s =>
        //            s.TypeVehicleId == vehicleTypeId &&
        //            s.Zones.ParkingId == parkingId)
        //        .ToListAsync();

        //    return sectors;
        //}
        public async Task<List<Sectors>> GetSectorsByVehicleTypeAsync(int vehicleTypeId, int parkingId)
        {
            var sectors = await _context.Sectors
                .Include(s => s.Slots)
                .Include(s => s.Zones) // importante para poder filtrar por ParkingId
                .Where(s =>
                    s.TypeVehicleId == vehicleTypeId &&
                    s.Zones.ParkingId == parkingId)
                .AsNoTracking() // 👈 evita el seguimiento de EF
                .ToListAsync();

            return sectors;
        }
    }
}
