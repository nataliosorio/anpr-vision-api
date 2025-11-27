using AutoMapper;
using Data.Interfaces.Parameter;
using Entity.Contexts;
using Entity.Contexts.parking;
using Entity.Dtos.Dashboard;
using Entity.Dtos.Parameter;
using Entity.Models.Parameter;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Utilities.Audit.Services;
using Utilities.Interfaces;

namespace Data.Implementations.Parameter
{
    public class SlotsData : RepositoryData<Slots>, ISlotsData
    {
        public SlotsData(ApplicationDbContext context, IConfiguration configuration, IAuditService auditService, ICurrentUserService currentUserService, IMapper mapper, IParkingContext parkingContext)
            : base(context, configuration, auditService, currentUserService, mapper, parkingContext)
        {

        }

        //public async Task<IEnumerable<SlotsDto>> GetAllJoinAsync()
        //{
        //    return await _context.Slots
        //        .AsNoTracking()
        //        .Select(p => new SlotsDto
        //        {
        //            // --- BaseDto ---
        //            Id = p.Id,                      // int? en BaseDto
        //            Asset = p.Asset,                 // bool? en BaseDto
        //            IsDeleted = p.IsDeleted,         // bool en BaseDto

        //            // --- GenericDto ---
        //            Name = p.Name,                   // string en GenericDto

        //            // --- SectorsDto ---
        //            IsAvailable = p.IsAvailable,
        //            SectorsId = p.SectorsId,
        //            Sectors = p.Sectors != null
        //                ? p.Sectors.Name
        //                : null
        //        })
        //        .ToListAsync();
        //}

        public async Task<IEnumerable<SlotsDto>> GetAllJoinAsync()
        {
            var parkingId = _parkingContext.ParkingId;

            var query = _context.Slots
                .AsNoTracking()
                .Where(s => s.IsDeleted != true); 

            if (parkingId.HasValue)
            {
                query = query.Where(s =>
                    s.Sectors != null &&
                    s.Sectors.Zones != null &&
                    s.Sectors.Zones.ParkingId == parkingId.Value
                );
            }

            return await query
                .Select(s => new SlotsDto
                {
                    Id = s.Id,
                    Asset = s.Asset,
                    IsDeleted = s.IsDeleted,
                    Name = s.Name,
                    IsAvailable = s.IsAvailable,
                    SectorsId = s.SectorsId,
                    Sectors = s.Sectors != null ? s.Sectors.Name : null
                })
                .ToListAsync();
        }



        public async Task<IEnumerable<Slots>> GetAllBySectorId(int sectorId)
        {
            return await _context.Slots
                .AsNoTracking()
                .Where(s => s.SectorsId == sectorId && s.IsDeleted == false)
                .ToListAsync();
        }

        public Task<bool> AnyAsync(Expression<Func<Slots, bool>> predicate)
       => _context.Slots.AsNoTracking().AnyAsync(predicate);

      
        public Task<int> CountExistingBySectorAsync(int sectorId)
        {
            return _context.Slots
                .Where(s => s.SectorsId == sectorId && s.IsDeleted != true)
                .CountAsync();
        }

        public async Task<OccupancyDto> GetOccupancyGlobalAsync()
        {
            // Total de slots NO eliminados (null = no eliminado)
            var total = await _context.Slots
                .AsNoTracking()
                .CountAsync(s => s.IsDeleted != true);

            if (total == 0) return new OccupancyDto { Occupied = 0, Total = 0, Percentage = 0 };

            // Ocupados = slots que tienen un RV abierto (ExitDate null) con SlotsId asignado
            var occupied = await _context.RegisteredVehicles
                .AsNoTracking()
                .Where(rv => rv.ExitDate == null && rv.SlotsId != null)
                .Select(rv => rv.SlotsId!.Value)
                .Distinct()
                .CountAsync();

            return new OccupancyDto
            {
                Occupied = occupied,
                Total = total,
                Percentage = Math.Round((double)occupied / total * 100, 2)
            };
        }

        public async Task<IEnumerable<SlotsDto>> GetAllByParkingIdAsync(int parkingId)
        {
            return await _context.Slots
                .AsNoTracking()
                .Where(s => s.Sectors != null &&
                            s.Sectors.Zones != null &&
                            s.Sectors.Zones.ParkingId == parkingId &&
                            (s.IsDeleted == null || s.IsDeleted == false))
                .Select(s => new SlotsDto
                {
                    Id = s.Id,
                    Name = s.Name,
                    IsAvailable = s.IsAvailable,
                    Asset = s.Asset,
                    IsDeleted = s.IsDeleted,
                    SectorsId = s.SectorsId,
                    Sectors = s.Sectors != null ? s.Sectors.Name : null,
                    
                })
                .ToListAsync();
        }


    }
}
