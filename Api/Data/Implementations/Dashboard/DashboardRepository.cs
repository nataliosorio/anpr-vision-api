using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data.Interfaces.Dashboard;
using Entity.Contexts;
using Entity.Dtos.Dashboard;
using Entity.Models.Parameter;
using Microsoft.EntityFrameworkCore;

namespace Data.Implementations.Dashboard
{
    /// <summary>
    /// Implementación del repositorio de Dashboard.
    /// 
    /// Esta clase contiene la lógica de acceso a datos para consultas estadísticas
    /// y métricas del sistema de parqueaderos (ocupación, distribución de tipos de vehículo, etc.),
    /// utilizando Entity Framework Core y consultas LINQ optimizadas con AsNoTracking().
    /// </summary>
    public class DashboardRepository : IDashboardRepository
    {
        private readonly ApplicationDbContext _db;
        public DashboardRepository(ApplicationDbContext db) => _db = db;

        /// <summary>
        /// Obtiene el total de vehículos actualmente estacionados en un parqueadero específico.
        /// </summary>
        /// <param name="parkingId">Identificador del parqueadero.</param>
        /// <returns>Número total de vehículos activos (sin fecha de salida) en el parqueadero.</returns>
        public async Task<int> GetTotalCurrentlyParkedByParkingAsync(int parkingId)
        {
            return await _db.RegisteredVehicles
                .AsNoTracking()
                .Where(rv => rv.ExitDate == null &&
                             rv.SlotsId != null &&
                             rv.Slots.Sectors.Zones.ParkingId == parkingId)
                .CountAsync();
        }
        /// <summary>
        /// Obtiene el total global de vehículos actualmente estacionados en todos los parqueaderos.
        /// </summary>
        /// <returns>Número total de vehículos activos globalmente.</returns>
        public async Task<int> GetTotalCurrentlyParkedGlobalAsync()
        {
            return await _db.RegisteredVehicles
                .AsNoTracking()
                .Where(rv => rv.ExitDate == null)
                .CountAsync();
        }
        /// <summary>
        /// Obtiene la distribución de tipos de vehículo (carros, motos, etc.)
        /// actualmente estacionados en un parqueadero específico.
        /// </summary>
        /// <param name="parkingId">Identificador del parqueadero.</param>
        /// <param name="includeZeros">Indica si se deben incluir tipos con conteo igual a cero.</param>
        /// <returns>Objeto <see cref="VehicleTypeDistributionDto"/> con totales, porcentajes y etiquetas.</returns>
        public async Task<VehicleTypeDistributionDto> GetVehicleTypeDistributionByParkingAsync(int parkingId, bool includeZeros = true)
        {
          

            // Vehículos actualmente estacionados en el parking
            var open = _db.RegisteredVehicles
                .AsNoTracking()
                .Where(rv => rv.ExitDate == null &&
                             rv.SlotsId != null &&
                             rv.Slots.Sectors.Zones.ParkingId == parkingId);

            // Agrupar por el tipo de vehículo asociado al sector del slot
            var counts = await open
                .GroupBy(rv => new
                {
                    rv.Slots.Sectors.TypeVehicleId,
                    rv.Slots.Sectors.TypeVehicle.Name
                })
                .Select(g => new
                {
                    g.Key.TypeVehicleId,
                    g.Key.Name,
                    Count = g.Count()
                })
                .ToListAsync();

            var total = counts.Sum(x => x.Count);

            // Obtener tipos de vehículo activos que existan en ese parking (por sus sectores)
            var types = await _db.Sectors
                .AsNoTracking()
                .Where(s => s.IsDeleted != true &&
                            s.Zones.ParkingId == parkingId &&
                            s.TypeVehicle.IsDeleted != true)
                .Select(s => new { s.TypeVehicle.Id, s.TypeVehicle.Name })
                .Distinct()
                .ToListAsync();

            var byId = counts.ToDictionary(c => c.TypeVehicleId, c => c.Count);
            var slices = new List<VehicleTypeSliceDto>();

            foreach (var t in types)
            {
                var c = byId.TryGetValue(t.Id, out var v) ? v : 0;
                if (!includeZeros && c == 0) continue;

                var pct = total == 0 ? 0 : Math.Round((double)c / Math.Max(1, total) * 100, 2);
                slices.Add(new VehicleTypeSliceDto
                {
                    TypeVehicleId = t.Id,
                    Name = t.Name,
                    Count = c,
                    Percentage = pct
                });
            }

            // Orden alfabético (ya sin el diccionario de orden fijo)
            slices = slices.OrderBy(s => s.Name).ToList();

            return new VehicleTypeDistributionDto
            {
                Total = total,
                Labels = slices.Select(s => s.Name).ToList(),
                Series = slices.Select(s => s.Count).ToList(),
                Slices = slices
            };
        }


        /// <summary>
        /// Obtiene la ocupación de cada sector dentro de una zona específica,
        /// mostrando totales, ocupados y porcentaje de ocupación.
        /// </summary>
        /// <param name="zoneId">Identificador de la zona.</param>
        /// <returns>Lista de <see cref="OccupancyItemDto"/> con la ocupación por sector.</returns>
        /// <exception cref="ArgumentException">Si <paramref name="zoneId"/> es inválido (≤ 0).</exception>
        public async Task<List<OccupancyItemDto>> GetSectorOccupancyByZoneAsync(int zoneId)
        {
            if (zoneId <= 0)
                throw new ArgumentException("zoneId inválido.");

            var totalsBySector = await _db.Slots
                .AsNoTracking()
                .Where(s => s.IsDeleted != true && s.Sectors.Zones.Id == zoneId)
                .GroupBy(s => new { s.SectorsId, s.Sectors.Name })
                .Select(g => new { g.Key.SectorsId, g.Key.Name, Total = g.Count() })
                .ToListAsync();

            var occupiedBySector = await _db.RegisteredVehicles
                .AsNoTracking()
                .Where(rv => rv.ExitDate == null &&
                             rv.SlotsId != null &&
                             rv.Slots.Sectors.Zones.Id == zoneId)
                .Select(rv => new { rv.Slots.SectorsId, rv.Slots.Sectors.Name })
                .Distinct()
                .GroupBy(x => new { x.SectorsId, x.Name })
                .Select(g => new { g.Key.SectorsId, g.Key.Name, Occupied = g.Count() })
                .ToListAsync();

            var occDict = occupiedBySector.ToDictionary(x => x.SectorsId, x => x.Occupied);
            var result = new List<OccupancyItemDto>();

            foreach (var t in totalsBySector)
            {
                var occ = occDict.TryGetValue(t.SectorsId, out var v) ? v : 0;
                result.Add(new OccupancyItemDto
                {
                    Id = t.SectorsId,
                    Name = t.Name,
                    Total = t.Total,
                    Occupied = occ,
                    Percentage = t.Total == 0 ? 0 : Math.Round((double)occ / t.Total * 100, 2)
                });
            }

            return result.OrderBy(x => x.Name).ToList();
        }
        /// <summary>
        /// Obtiene los datos de ocupación global (total de slots, ocupados, porcentaje)
        /// para un parqueadero específico.
        /// </summary>
        /// <param name="parkingId">Identificador del parqueadero.</param>
        /// <returns>Un objeto <see cref="OccupancyDto"/> con los datos de ocupación global.</returns>
        public async Task<OccupancyDto> GetOccupancyGlobalAsync(int parkingId)
        {
            if (parkingId <= 0) throw new ArgumentException("parkingId inválido.");

            // Total de slots en el parking
            var total = await _db.Slots
                .AsNoTracking()
                .CountAsync(s => s.IsDeleted != true &&
                                s.Sectors.Zones.ParkingId == parkingId);

            if (total == 0) return new OccupancyDto { Occupied = 0, Total = 0, Percentage = 0 };

            // Ocupados = contar los slots del parking que tienen al menos un RegisteredVehicle abierto
            var occupied = await _db.Slots
                .AsNoTracking()
                .Where(s => s.IsDeleted != true &&
                            s.Sectors.Zones.ParkingId == parkingId &&
                            _db.RegisteredVehicles.Any(rv => rv.SlotsId == s.Id && rv.ExitDate == null))
                .CountAsync();

            return new OccupancyDto
            {
                Occupied = occupied,
                Total = total,
                Percentage = Math.Round((double)occupied / total * 100, 2)
            };
        }

    }
}
