using AutoMapper;
using Business.Interfaces;
using Business.Interfaces.Operational;
using Data.Implementations;
using Data.Interfaces.Operational;
using Data.Interfaces.Parameter;
using Entity.Dtos.Operational;
using Entity.Enums;
using Entity.Models;
using Entity.Models.Operational;
using Entity.Models.Parameter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using Utilities.Exceptions;
using Utilities.Helpers.Validators;

namespace Business.Implementations.Operational
{

    public class VehicleBusiness : RepositoryBusiness<Vehicle, VehicleDto>, IVehicleBusiness
    {
        private readonly IVehicleData _data;
        private readonly IMapper _mapper;
        private readonly IRegisteredVehiclesData _registeredVehicleData; // Data para RegisteredVehicles
        private readonly ISectorsData _sectorData; // Data para sectores y slots
        private readonly ISlotsData _slotsData;

        public VehicleBusiness(IVehicleData data, IMapper mapper, IRegisteredVehiclesData registeredVehicleData, ISectorsData sectorData, ISlotsData slotsData)
            : base(data, mapper)
        {
            _data = data;
            _mapper = mapper;
            _registeredVehicleData = registeredVehicleData;
            _sectorData = sectorData;
            _slotsData = slotsData;
        }
        public async Task<IEnumerable<VehicleDto>> GetAllJoinAsync()
        {
            try
            {
                IEnumerable<VehicleDto> entities = await _data.GetAllJoinAsync();
                if (!entities.Any()) throw new InvalidOperationException("No se encontraron vehiculos");
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
                throw new Exception("Error al obtener las vehiculos .", ex);
            }
        }

        public override async Task<VehicleDto> Save(VehicleDto dto)
        {
            try
            {
                // 🔹 Limpieza de strings
                dto.Plate = dto.Plate?.Trim().ToUpper() ?? "";
                dto.Color = dto.Color?.Trim();

                // 🔹 Validación de campos obligatorios
                Validations.ValidateDto(dto, "Plate", "TypeVehicleId", "ClientId");

                if (string.IsNullOrWhiteSpace(dto.Plate))
                    throw new ArgumentException("El campo 'Plate' es obligatorio.");

                // 🔹 Validación Color (opcional)
                if (!string.IsNullOrWhiteSpace(dto.Color) && dto.Color.Length > 30)
                    throw new ArgumentException("El color no puede superar los 30 caracteres.");

                // 🔹 Validación TypeVehicleId
                if (dto.TypeVehicleId <= 0)
                    throw new ArgumentException("Debe seleccionar un tipo de vehículo válido.");

                // 🔹 Validación ClientId
                if (dto.ClientId <= 0)
                    throw new ArgumentException("Debe seleccionar un cliente válido.");

                // 🔹 Validar que la placa no exista ya en la BD
                var exists = await _data.ExistsAsync(v => v.Plate.ToUpper() == dto.Plate.ToUpper());
                if (exists)
                    throw new ArgumentException($"Ya existe un vehículo registrado con la placa '{dto.Plate}'.");

                // 🔹 Guardar entidad
                dto.Asset = true;

                Vehicle entity = _mapper.Map<Vehicle>(dto);

                // 🔹 Guardar en la base de datos
                entity = await _data.Save(entity);

                VehicleDto entityDto = _mapper.Map<VehicleDto>(entity);
                // 🔹 Devolver DTO con solo IDs
                return entityDto;
            }
            catch (InvalidOperationException invOe)
            {
                throw new InvalidOperationException($"Error: {invOe.Message}", invOe);
            }
            catch (ArgumentException argEx)
            {
                throw new ArgumentException($"Error: {argEx.Message}");
            }
            catch (Exception ex)
            {
                throw new BusinessException("Error al registrar el vehículo.", ex);
            }
        }

        public async Task<RegisteredVehiclesDto?> GetActiveVehicleBySlotAsync(int slotId)
        {
            var registeredVehicle = await _data.GetActiveRegisteredVehicleBySlotAsync(slotId);

            if (registeredVehicle == null)
                return null;

            return new RegisteredVehiclesDto
            {
                Id = registeredVehicle.Id,
                SlotsId = registeredVehicle.SlotsId,
                VehicleId = registeredVehicle.Vehicle.Id,
                EntryDate = registeredVehicle.EntryDate
            };
        }

        public async Task<VehicleDto> GetVehicleByPlate(string plate)
        {
            plate = plate.Trim().ToUpper();
            Vehicle? vehicle = await _data.GetVehicleByPlate(plate);
            if(vehicle == null)
            {
                return null ;
            }
            VehicleDto returnVehicle = _mapper.Map<VehicleDto>(vehicle);
            return returnVehicle;
        }

    }
}
