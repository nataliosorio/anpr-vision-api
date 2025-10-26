using AutoMapper;
using Data.Interfaces.Parameter;
using Entity.Contexts;
using Entity.Contexts.parking;
using Entity.Dtos.Parameter;
using Entity.Models.Parameter;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Utilities.Audit.Services;
using Utilities.Interfaces;

namespace Data.Implementations.Parameter
{
    public class RatesData : RepositoryData<Rates>, IRatesData
    {
        public RatesData(
            ApplicationDbContext context,
            IConfiguration configuration,
            IAuditService auditService,
            ICurrentUserService currentUserService,
            IMapper mapper,
            IParkingContext parkingContext)
            : base(context, configuration, auditService, currentUserService, mapper, parkingContext)
        {
        }

        // ============================================================
        // 🔹 GetAllJoinAsync — aplica filtro automático de parking
        // ============================================================
        public async Task<IEnumerable<RatesDto>> GetAllJoinAsync()
        {
            var query = _context.Rates
                .Include(p => p.Parking)
                .Include(p => p.RatesType)
                .Include(p => p.TypeVehicle)
                .AsNoTracking();

            // ✅ Filtra automáticamente por parkingId si el usuario tiene contexto
            if (_parkingContext.ParkingId.HasValue)
            {
                var pid = _parkingContext.ParkingId.Value;
                query = query.Where(r => r.ParkingId == pid);
            }

            return await query
                .Select(p => new RatesDto
                {
                    // --- BaseDto ---
                    Id = p.Id,
                    Asset = p.Asset,
                    IsDeleted = p.IsDeleted,

                    // --- Datos de la tarifa ---
                    Name = p.Name,
                    Type = p.Type,
                    Amount = p.Amount,
                    StarHour = p.StarHour,
                    EndHour = p.EndHour,
                    Year = p.Year,

                    // --- ParkingDto ---
                    ParkingId = p.ParkingId,
                    Parking = p.Parking != null ? p.Parking.Name : null,

                    // --- RatesTypeDto ---
                    RatesTypeId = p.RatesTypeId,
                    RatesType = p.RatesType != null ? p.RatesType.Name : null,

                    // --- TypeVehicleDto ---
                    TypeVehicleId = p.TypeVehicleId,
                    TypeVehicle = p.TypeVehicle != null ? p.TypeVehicle.Name : null
                })
                .ToListAsync();
        }

        // ============================================================
        // 🔹 GetByParkingAsync — opcional (manual)
        // ============================================================
        //public async Task<IEnumerable<RatesDto>> GetByParkingAsync(int parkingId)
        //{
        //    return await _context.Rates
        //        .AsNoTracking()
        //        .Where(r => r.ParkingId == parkingId && (r.IsDeleted == false || r.IsDeleted == null))
        //        .Select(p => new RatesDto
        //        {
        //            Id = p.Id,
        //            Asset = p.Asset,
        //            IsDeleted = p.IsDeleted,

        //            Name = p.Name,
        //            Type = p.Type,
        //            Amount = p.Amount,
        //            StarHour = p.StarHour,
        //            EndHour = p.EndHour,
        //            Year = p.Year,

        //            ParkingId = p.ParkingId,
        //            Parking = p.Parking != null ? p.Parking.Name : null,

        //            RatesTypeId = p.RatesTypeId,
        //            RatesType = p.RatesType != null ? p.RatesType.Name : null,

        //            TypeVehicleId = p.TypeVehicleId,
        //            TypeVehicle = p.TypeVehicle != null ? p.TypeVehicle.Name : null
        //        })
        //        .ToListAsync();
        //}
    }
}
