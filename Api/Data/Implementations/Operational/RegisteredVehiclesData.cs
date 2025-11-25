using AutoMapper;
using Data.Interfaces.Operational;
using Entity.Contexts;
using Entity.Contexts.parking;
using Entity.Dtos.Dashboard;
using Entity.Dtos.Operational;
using Entity.Enums;
using Entity.Models.Operational;
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

namespace Data.Implementations.Operational
{
    public class RegisteredVehicleData : RepositoryData<RegisteredVehicles>, IRegisteredVehiclesData
    {
        public RegisteredVehicleData(ApplicationDbContext context, IConfiguration configuration, IAuditService auditService, ICurrentUserService currentUserService, IMapper mapper, IParkingContext parkingContext)
            : base(context, configuration, auditService, currentUserService, mapper, parkingContext)
        {

        }

        // Método que valida si un slot está ocupado
        public async Task<bool> AnyActiveRegisteredVehicleInSlotAsync(int slotId)
        {
            return await _context.RegisteredVehicles
                .AnyAsync(rv => rv.SlotsId == slotId && rv.ExitDate == null);
        }
        public async Task<IEnumerable<RegisteredVehiclesDto>> GetAllJoinAsync()
        {
            return await _context.RegisteredVehicles
                .AsNoTracking()
                .Where(p => p.IsDeleted == false)
                .Select(p => new RegisteredVehiclesDto
                {
                    // --- BaseDto ---
                    Id = p.Id,                      // int? en BaseDto
                    Asset = p.Asset,                 // bool? en BaseDto
                    IsDeleted = p.IsDeleted,         // bool en BaseDto

                    EntryDate = p.EntryDate,
                    ExitDate = p.ExitDate,


                    // --- ZonesDto ---
                    VehicleId = p.VehicleId,
                    Vehicle = p.Vehicle != null
                        ? p.Vehicle.Plate
                        : null,

                    //slots

                    SlotsId = p.SlotsId,
                    Slots = p.Slots != null

                    ? p.Slots.Name
                    : null

                })
                .ToListAsync();
                
        }

        // ---------- NUEVOS MÉTODOS ----------
        public async Task<int> GetTotalCurrentlyParkedByParkingAsync(int parkingId)
        {
            // Nota: se excluyen RV sin SlotsId porque no es posible inferir el parking con tu modelo actual.
            return await _context.RegisteredVehicles
                .AsNoTracking()
                .Where(rv => rv.ExitDate == null
                             && rv.SlotsId != null
                             && rv.Slots.Sectors.Zones.ParkingId == parkingId
                             /* && !rv.IsDeleted */)
                .CountAsync();
        }

        public Task<int> GetTotalCurrentlyParkedAsync()
        {
            // TOTAL GLOBAL (sin filtrar por parking)
            // Nota: aquí NO filtramos por SlotsId para incluir también los que aún no tienen plaza asignada.
            return _context.RegisteredVehicles
                .AsNoTracking()
                .Where(rv => rv.ExitDate == null
                             /* && !rv.IsDeleted */)
                .CountAsync();
        }

        public async Task<VehicleTypeDistributionDto> GetVehicleTypeDistributionGlobalAsync(bool includeZeros = true)
        {
            // Vehículos actualmente adentro (ExitDate NULL). No filtramos por Slots en global.
            var open = _context.RegisteredVehicles
                .AsNoTracking()
                .Where(rv => rv.ExitDate == null);

            // Contamos vehículos distintos por tipo (evita duplicados abiertos)
            var counts = await open
                .Select(rv => new { rv.VehicleId, rv.Vehicle.TypeVehicleId, rv.Vehicle.TypeVehicle.Name })
                .Distinct() // por VehicleId/Type
                .GroupBy(x => new { x.TypeVehicleId, x.Name })
                .Select(g => new { g.Key.TypeVehicleId, g.Key.Name, Count = g.Count() })
                .ToListAsync();

            var total = counts.Sum(x => x.Count);

            // Traemos catálogo de tipos para leyenda estable e incluir ceros si se pide
            var types = await _context.Set<TypeVehicle>()
                .AsNoTracking()
                .Where(tv => tv.IsDeleted != true)
                .Select(tv => new { tv.Id, tv.Name })
                .ToListAsync();

            var byId = counts.ToDictionary(c => c.TypeVehicleId, c => c.Count);
            var slices = new List<VehicleTypeSliceDto>();

            foreach (var t in types)
            {
                var c = byId.TryGetValue(t.Id, out var v) ? v : 0;
                if (!includeZeros && c == 0) continue;
                var pct = total == 0 ? 0 : Math.Round((double)c / Math.Max(1, total) * 100, 2);
                slices.Add(new VehicleTypeSliceDto { TypeVehicleId = t.Id, Name = t.Name, Count = c, Percentage = pct });
            }

            // Orden sugerido: Carro, Moto, Camión, luego alfabético
            var order = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase) { ["Carro"] = 1, ["Moto"] = 2, ["Camión"] = 3, ["Camion"] = 3 };
            slices = slices.OrderBy(s => order.TryGetValue(s.Name, out var o) ? o : 100)
                           .ThenBy(s => s.Name).ToList();

            return new VehicleTypeDistributionDto
            {
                Total = total,
                Labels = slices.Select(s => s.Name).ToList(),
                Series = slices.Select(s => s.Count).ToList(),
                Slices = slices
            };
        }

        public async Task<List<OccupancyItemDto>> GetSectorOccupancyByZoneAsync(int zoneId)
        {
            if (zoneId <= 0) throw new ArgumentException("zoneId inválido.");

            // 1) Totales de slots por SECTOR (NO eliminados) dentro de la zona
            var totalsBySector = await _context.Slots
                .AsNoTracking()
                .Where(s => s.IsDeleted != true && s.Sectors.Zones.Id == zoneId)
                .GroupBy(s => new { SectorId = s.SectorsId, SectorName = s.Sectors.Name })
                .Select(g => new { g.Key.SectorId, g.Key.SectorName, Total = g.Count() })
                .ToListAsync();

            // 2) Slots OCUPADOS por SECTOR en esa zona (distinct SlotsId)
            var occupiedBySector = await _context.RegisteredVehicles
                .AsNoTracking()
                .Where(rv => rv.ExitDate == null
                          && rv.SlotsId != null
                          && rv.Slots.IsDeleted != true
                          && rv.Slots.Sectors.Zones.Id == zoneId)
                .Select(rv => new { rv.SlotsId, SectorId = rv.Slots.SectorsId, SectorName = rv.Slots.Sectors.Name })
                .Distinct()
                .GroupBy(x => new { x.SectorId, x.SectorName })
                .Select(g => new { g.Key.SectorId, g.Key.SectorName, Occupied = g.Count() })
                .ToListAsync();

            var occDict = occupiedBySector.ToDictionary(x => x.SectorId, x => x.Occupied);
            var result = new List<OccupancyItemDto>();

            foreach (var t in totalsBySector)
            {
                var occ = occDict.TryGetValue(t.SectorId, out var v) ? v : 0;
                result.Add(new OccupancyItemDto
                {
                    Id = t.SectorId,
                    Name = t.SectorName,
                    Total = t.Total,
                    Occupied = occ,
                    Percentage = t.Total == 0 ? 0 : Math.Round((double)occ / t.Total * 100, 2)
                });
            }

            return result.OrderBy(x => x.Name).ToList();
        }

        public async Task<IEnumerable<RegisteredVehiclesDto>> GetByParkingAsync(int parkingId)
        {
            return await _context.RegisteredVehicles
                .AsNoTracking()
                .Where(rv =>
                    rv.ExitDate == null &&
                    rv.SlotsId != null &&
                    rv.Slots.Sectors.Zones.ParkingId == parkingId &&
                    (rv.IsDeleted == false || rv.IsDeleted == null))
                .Select(rv => new RegisteredVehiclesDto
                {
                    Id = rv.Id,
                    Asset = rv.Asset,
                    IsDeleted = rv.IsDeleted,
                    EntryDate = rv.EntryDate,
                    ExitDate = rv.ExitDate,

                    VehicleId = rv.VehicleId,
                    Vehicle = rv.Vehicle != null ? rv.Vehicle.Plate : null,

                    SlotsId = rv.SlotsId,
                    Slots = rv.Slots != null ? rv.Slots.Name : null
                })
                .ToListAsync();
        }

       
        public async Task<RegisteredVehicles?> GetActiveRegisterByVehicleIdAsync(int vehicleId)
        {
            return await _context.RegisteredVehicles
                .Include(rv => rv.Slots)
                .Include(rv => rv.Vehicle)
                .Where(rv => rv.VehicleId == vehicleId &&
                             rv.Status == ERegisterStatus.In &&
                             rv.ExitDate == null &&
                             rv.Asset == true)
                .AsNoTracking() // 👈 agregado
                .FirstOrDefaultAsync();
        }

        public async Task<RegisteredVehicles?> GetFullByIdAsync(int id)
        {
            return await _context.RegisteredVehicles
                .Include(rv => rv.Vehicle)
                    .ThenInclude(v => v.TypeVehicle)
                .Include(rv => rv.Slots)
                    .ThenInclude(s => s.Sectors)
                .AsNoTracking()
                .FirstOrDefaultAsync(rv => rv.Id == id);
        }

    }
}
