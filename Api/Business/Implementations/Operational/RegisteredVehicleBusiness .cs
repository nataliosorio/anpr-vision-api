using AutoMapper;
using Business.Implementations.Parameter;
using Business.Interfaces;
using Business.Interfaces.Operational;
using Business.Interfaces.Parameter;
using Data.Implementations;
using Data.Implementations.Operational;
using Data.Interfaces.Operational;
using Entity.Dtos.Dashboard;
using Entity.Dtos.Operational;
using Entity.Dtos.Parameter;
using Entity.Enums;
using Entity.Models.Operational;
using Entity.Models.Parameter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities.Exceptions;
using Utilities.Helpers.Validators;

namespace Business.Implementations.Operational
{
   
    public class RegisteredVehicleBusiness : RepositoryBusiness<RegisteredVehicles, RegisteredVehiclesDto>, IRegisteredVehicleBusiness
    {
        private readonly IRegisteredVehiclesData _data;
        private readonly IVehicleBusiness _vehicleBusiness;
        private readonly ISectorsBusiness _sectorsBusiness;
        private readonly ISlotsBusiness _slotsBusiness;
        private readonly IMapper _mapper;
        public RegisteredVehicleBusiness(IRegisteredVehiclesData data, IMapper mapper, IVehicleBusiness vehicleBusiness, ISectorsBusiness sectorsBusiness, ISlotsBusiness slotsBusiness)
            : base(data, mapper)
        {
            _data = data;
            _vehicleBusiness = vehicleBusiness;
            _sectorsBusiness = sectorsBusiness;
            _mapper = mapper;
            _slotsBusiness = slotsBusiness;
        }


        public async Task<IEnumerable<RegisteredVehiclesDto>> GetAllJoinAsync()
        {
            try
            {
                IEnumerable<RegisteredVehiclesDto> entities = await _data.GetAllJoinAsync();
                if (!entities.Any()) throw new InvalidOperationException("No se encontraron registros de vehiculos.");
                return entities;
            }
            catch (InvalidOperationException invEx)
            {
                throw new InvalidOperationException("error: ", invEx);
            }
            catch (ArgumentException argEx)
            {
                throw new ArgumentException("error: ", argEx);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener las registros .", ex);
            }
        }

        // ---------- NUEVOS MÉTODOS ----------
        public async Task<int> GetTotalCurrentlyParkedByParkingAsync(int parkingId)
        {
            if (parkingId <= 0) throw new ArgumentException("parkingId inválido.");
            try
            {
                return await _data.GetTotalCurrentlyParkedByParkingAsync(parkingId);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener el total de vehículos estacionados por parking.", ex);
            }
        }

        public async Task<int> GetTotalCurrentlyParkedAsync()
        {
            try
            {
                return await _data.GetTotalCurrentlyParkedAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener el total de vehículos estacionados (global).", ex);
            }
        }

        public Task<VehicleTypeDistributionDto> GetVehicleTypeDistributionGlobalAsync(bool includeZeros = true)
        => _data.GetVehicleTypeDistributionGlobalAsync(includeZeros);

        public Task<List<OccupancyItemDto>> GetSectorOccupancyByZoneAsync(int zoneId)
        => _data.GetSectorOccupancyByZoneAsync(zoneId);

        public async Task<IEnumerable<RegisteredVehiclesDto>> GetByParkingAsync(int parkingId)
        {
            try
            {
                if (parkingId <= 0)
                    throw new ArgumentException("Debe especificar un ID de parqueadero válido.");

                var data = await _data.GetByParkingAsync(parkingId);

                if (!data.Any())
                    throw new InvalidOperationException("No se encontraron registros para este parqueadero.");

                return data;
            }
            catch (ArgumentException argEx)
            {
                throw new ArgumentException($"Error: {argEx.Message}");
            }
            catch (InvalidOperationException invEx)
            {
                throw new InvalidOperationException($"Error: {invEx.Message}", invEx);
            }
            catch (Exception ex)
            {
                throw new BusinessException("Error al obtener los vehículos por parqueadero.", ex);
            }
        }

        // Método para registrar vehículo y asignar slot
        public async Task<RegisteredVehiclesDto> RegisterVehicleWithSlotAsync(int vehicleId, int parkingId)
        {
            // 1Obtener el vehículo existente
            VehicleDto vehicle = await _vehicleBusiness.GetById(vehicleId) ?? throw new Exception("Vehículo no encontrado.");

            //  Obtener sectores compatibles con el tipo de vehículo
            Slots assignedSlot = await _slotsBusiness.AssignAvailableSlotAsync(vehicle.TypeVehicleId, parkingId);

            // 6. Marcar el slot como ocupado
            assignedSlot.IsAvailable = false;
            SlotsDto assignedSlotDto = _mapper.Map<SlotsDto>(assignedSlot);
            await _slotsBusiness.Update(assignedSlotDto);


            //  Crear RegisteredVehicle
            RegisteredVehicles registeredVehicle = new RegisteredVehicles
            {
                VehicleId = vehicle.Id,
                SlotsId = assignedSlot.Id,
                EntryDate = DateTime.UtcNow,
                Status = ERegisterStatus.In,
                Asset = true
            };

            await _data.Save(registeredVehicle);

            RegisteredVehiclesDto returnRegisteredVehicle = _mapper.Map<RegisteredVehiclesDto>(registeredVehicle);

            returnRegisteredVehicle.Slots = assignedSlot.Name;

            return returnRegisteredVehicle;
        }
        public async Task<RegisteredVehiclesDto> RegisterVehicleExitAsync(int vehicleId)
        {
            //  Buscar un registro activo del vehículo
            RegisteredVehicles? activeRegister = await _data.GetActiveRegisterByVehicleIdAsync(vehicleId) ?? throw new BusinessException("No se encontró una entrada activa para este vehículo.");

            //  Marcar salida
            activeRegister.ExitDate = DateTime.UtcNow;
            activeRegister.Status = ERegisterStatus.Out;

            //  Liberar el slot
            if (activeRegister.SlotsId.HasValue)
            {
                SlotsDto slot = await _slotsBusiness.GetById(activeRegister.SlotsId.Value);
                if (slot != null)
                {
                    slot.IsAvailable = true;
                    await _slotsBusiness.Update(slot);
                }
            }

            //  Guardar cambios
            await _data.Update(activeRegister);

            //  Mapear y devolver DTO
            RegisteredVehiclesDto dto = _mapper.Map<RegisteredVehiclesDto>(activeRegister);
            dto.Slots = (await _slotsBusiness.GetById(activeRegister.SlotsId ?? 0))?.Name ?? "N/A";
            return dto;
        }


    }
}
