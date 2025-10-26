using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Business.Interfaces.Dashboard;
using Data.Interfaces.Dashboard;
using Entity.Dtos.Dashboard;
using Microsoft.Extensions.Logging;

namespace Business.Implementations.Dashboard
{
    /// <summary>
    /// Implementación de la capa de negocio para la obtención de datos del Dashboard.
    /// 
    /// Esta clase actúa como intermediario entre la capa de presentación (controladores)
    /// y la capa de datos (<see cref="IDashboardRepository"/>), aplicando validaciones
    /// y reglas de negocio antes de acceder al repositorio.
    /// </summary>
    public class DashboardBusiness : IDashboardBusiness
    {
        private readonly IDashboardRepository _repo;
        private readonly ILogger<DashboardBusiness> _logger;
        public DashboardBusiness(IDashboardRepository repo, ILogger<DashboardBusiness> logger)
        {
            _repo = repo;
            _logger = logger;
        }


        /// <summary>
        /// Obtiene el total de vehículos actualmente estacionados en un parqueadero específico.
        /// </summary>
        /// <param name="parkingId">Identificador único del parqueadero.</param>
        /// <returns>Un entero que representa el número total de vehículos estacionados actualmente.</returns>
        /// <exception cref="ArgumentException">Se lanza si <paramref name="parkingId"/> es menor o igual a cero.</exception>
        public async Task<int> GetTotalCurrentlyParkedByParkingAsync(int parkingId)
        {
            if (parkingId <= 0) throw new ArgumentException("parkingId inválido.");
            return await _repo.GetTotalCurrentlyParkedByParkingAsync(parkingId);
        }
        /// <summary>
        /// Obtiene el total global de vehículos actualmente estacionados en todos los parqueaderos.
        /// </summary>
        /// <returns>Un entero con el total de vehículos estacionados globalmente.</returns>
        public async Task<int> GetTotalCurrentlyParkedGlobalAsync()
        {
            return await _repo.GetTotalCurrentlyParkedGlobalAsync();
        }

        /// <summary>
        /// Obtiene la distribución de tipos de vehículos (carros, motos, etc.) en un parqueadero determinado.
        /// </summary>
        /// <param name="parkingId">Identificador del parqueadero.</param>
        /// <param name="includeZeros">Indica si se deben incluir los tipos con conteo cero en los resultados.</param>
        /// <returns>Un objeto <see cref="VehicleTypeDistributionDto"/> con la distribución por tipo de vehículo.</returns>
        /// <exception cref="ArgumentException">Se lanza si <paramref name="parkingId"/> es menor o igual a cero.</exception>
        public async Task<VehicleTypeDistributionDto> GetVehicleTypeDistributionGlobalAsync(int parkingId, bool includeZeros = true)
        {
            if (parkingId <= 0) throw new ArgumentException("parkingId inválido.");
            return await _repo.GetVehicleTypeDistributionByParkingAsync(parkingId,includeZeros);
        }
        /// <summary>
        /// Obtiene el porcentaje de ocupación por sector dentro de una zona específica.
        /// </summary>
        /// <param name="zoneId">Identificador único de la zona.</param>
        /// <returns>Una lista de <see cref="OccupancyItemDto"/> con el detalle de ocupación por sector.</returns>
        /// <exception cref="ArgumentException">Se lanza si <paramref name="zoneId"/> es menor o igual a cero.</exception>
        public async Task<List<OccupancyItemDto>> GetSectorOccupancyByZoneAsync(int zoneId)
        {
            if (zoneId <= 0) throw new ArgumentException("zoneId inválido.");
            return await _repo.GetSectorOccupancyByZoneAsync(zoneId);
        }

        /// <summary>
        /// Obtiene la información de ocupación global (puestos disponibles, ocupados, porcentaje de ocupación, etc.)
        /// de un parqueadero específico.
        /// </summary>
        /// <param name="parkingId">Identificador del parqueadero.</param>
        /// <returns>Un objeto <see cref="OccupancyDto"/> con los datos de ocupación global.</returns>
        /// <exception cref="ArgumentException">Se lanza si <paramref name="parkingId"/> es menor o igual a cero.</exception>

        public async Task<OccupancyDto> GetOccupancyGlobalAsync(int parkingId)
        {
            if (parkingId <= 0) throw new ArgumentException("parkingId inválido");
            return await _repo.GetOccupancyGlobalAsync(parkingId);
        }

      
    }
}
