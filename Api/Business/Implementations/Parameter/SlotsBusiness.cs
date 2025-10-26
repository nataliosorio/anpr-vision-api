using AutoMapper;
using Business.Interfaces.Operational;
using Business.Interfaces.Parameter;
using Data.Interfaces;
using Data.Interfaces.Operational;
using Data.Interfaces.Parameter;
using Entity.Dtos.Dashboard;
using Entity.Dtos.Parameter;
using Entity.Models.Parameter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities.Exceptions;
using Utilities.Helpers.Validators;

namespace Business.Implementations.Parameter
{

    public class SlotsBusiness : RepositoryBusiness<Slots, SlotsDto>, ISlotsBusiness
    {
        private readonly ISlotsData _data;
        private readonly IMapper _mapper;
        private readonly ISectorsBusiness _sectorsBusiness;
        private readonly IRegisteredVehiclesData _registeredVehicleData;

        public SlotsBusiness(ISlotsData data, IMapper mapper, ISectorsBusiness sectors, IRegisteredVehiclesData registeredVehicle)
            : base(data, mapper)
        {
            _data = data;
            _mapper = mapper;
            _sectorsBusiness = sectors;
            _registeredVehicleData = registeredVehicle;
        }


        public async Task<IEnumerable<SlotsDto>> GetAllJoinAsync()
        {
            try
            {
                IEnumerable<SlotsDto> entities = await _data.GetAllJoinAsync();
                if (!entities.Any()) throw new InvalidOperationException("No se encontraron slots.");
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
                throw new Exception("Error al obtener los slots.", ex);
            }
        }

        public async Task<IEnumerable<SlotsDto>> GetAllBySectorId(int sectorId)
        {
            try
            {
                if (sectorId < 1) throw new ArgumentException("El id del sector es invalido.");
                IEnumerable<Slots> entities = await _data.GetAllBySectorId(sectorId);
                if (!entities.Any()) throw new InvalidOperationException("No se encontraron slots para el sector seleccionado.");
                return _mapper.Map<IEnumerable<SlotsDto>>(entities);
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
                throw new Exception("Error al obtener los slots del sector.", ex);
            }
        }
        public override async Task<SlotsDto> Save(SlotsDto dto)
        {
            try
            {
                // Validaciones básicas (lanza ArgumentException si falta algún campo)
                Validations.ValidateDto(dto, "IsAvailable", "SectorsId");
                if (dto.SectorsId <= 0)
                    throw new ArgumentException("El campo SectorsId debe ser mayor a 0.");

                // 1) El sector debe existir y NO estar eliminado (null-safe)
                var sector = await _sectorsBusiness.GetById(dto.SectorsId);
                if (sector == null)
                    throw new InvalidOperationException($"El sector con Id {dto.SectorsId} no existe.");

                if (sector.IsDeleted.GetValueOrDefault(false))
                    throw new InvalidOperationException(
                        $"El sector '{sector.Name}' está eliminado lógicamente. No puedes crear slots asociados a un sector eliminado."
                    );

                // Defaults para evitar tri-estado (null-coalescing assignment)
                //if (dto.Asset == null) dto.Asset = true;
                //if (dto.IsDeleted == null) dto.IsDeleted = false;

                dto.Asset = true;
                dto.IsDeleted = false;

                // 2) Dedupe por nombre (solo NO eliminados — null-safe)
                var existeDuplicado = await _data.AnyAsync(s =>
                    s.SectorsId == dto.SectorsId
                    && s.Name == dto.Name
                    && !(s.IsDeleted ?? false)   // true => eliminado; ! => no eliminado
                );

                if (existeDuplicado)
                    throw new InvalidOperationException("Ya existe un slot con ese nombre en el mismo sector.");

                // 3) Capacidad: cuentan TODOS los NO eliminados — null-safe
                bool nuevoCuenta = dto.IsDeleted.GetValueOrDefault(false) == false; // null -> false -> cuenta
                if (nuevoCuenta)
                {
                    var existentes = await _data.CountExistingBySectorAsync(dto.SectorsId);
                    int capacidad = sector.Capacity;
                    if (existentes >= capacidad)
                        throw new InvalidOperationException(
                            $"Capacidad del sector ({capacidad}) excedida. No puedes crear más slots en el sector {sector.Name}."
                        );
                }

                // 4) Guardado: mapear manualmente para evitar problemas de mapeo automático
                var entity = new Slots
                {
                    Name = dto.Name,
                    SectorsId = dto.SectorsId,
                    IsAvailable = dto.IsAvailable,
                    Asset = dto.Asset.GetValueOrDefault(true),
                    IsDeleted = dto.IsDeleted.GetValueOrDefault(false)
                    // agrega aquí otras propiedades simples si aplican
                };

                entity = await _data.Save(entity);

                // Devolver DTO con los datos persistidos
                return new SlotsDto
                {
                    Id = entity.Id,
                    Name = entity.Name,
                    SectorsId = entity.SectorsId,
                    IsAvailable = entity.IsAvailable,
                    Asset = entity.Asset,
                    IsDeleted = entity.IsDeleted
                    // completa con otras propiedades si las usas
                };
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
                // imprime traza en consola para diagnóstico (opcional, quitar en prod)
                Console.WriteLine("Exception en SlotsBusiness.Save:");
                Console.WriteLine(ex.ToString());

                throw new BusinessException("Error al crear el registro del slot.", ex);
            }
        }


        public override async Task<SlotsDto> Update(SlotsDto dto)
        {
            try
            {
                Validations.ValidateDto(dto, "Id", "IsAvailable", "SectorsId");
                if (dto.SectorsId <= 0)
                    throw new ArgumentException("El campo SectorsId debe ser mayor a 0.");

                var actual = await _data.GetById(dto.Id);
                if (actual == null)
                    throw new InvalidOperationException($"El slot con Id {dto.Id} no existe.");

                var sectorDestino = await _sectorsBusiness.GetById(dto.SectorsId);
                if (sectorDestino == null)
                    throw new InvalidOperationException($"El sector con Id {dto.SectorsId} no existe.");

                // ❗ No permitir slot NO eliminado en un sector eliminado (null-safe)
                bool dtoNoEliminado = dto.IsDeleted.GetValueOrDefault(false) == false;
                bool sectorDestinoEliminado = sectorDestino.IsDeleted.GetValueOrDefault(false);
                if (dtoNoEliminado && sectorDestinoEliminado)
                    throw new InvalidOperationException(
                        $"El sector '{sectorDestino.Name}' está eliminado lógicamente. No puedes asociar un slot no eliminado a un sector eliminado."
                    );

                if (dto.Asset == null) dto.Asset = true;
                if (dto.IsDeleted == null) dto.IsDeleted = false;

                // Dedupe por nombre (no eliminados) excluyendo el propio Id — null-safe
                var existeDuplicado = await _data.AnyAsync(s =>
                    s.Id != dto.Id
                    && s.SectorsId == dto.SectorsId
                    && s.Name == dto.Name
                    && !(s.IsDeleted ?? false)
                );
                if (existeDuplicado)
                    throw new InvalidOperationException("Ya existe un slot con ese nombre en el mismo sector.");

                // Capacidad: cuentan NO eliminados — null-safe
                bool antesContaba = actual.IsDeleted.GetValueOrDefault(false) == false;
                bool ahoraCuenta = dto.IsDeleted.GetValueOrDefault(false) == false;
                bool cambiaSector = actual.SectorsId != dto.SectorsId;

                if (ahoraCuenta && (cambiaSector || !antesContaba))
                {
                    var existentesEnDestino = await _data.CountExistingBySectorAsync(dto.SectorsId);
                    int capacidad = sectorDestino.Capacity;

                    if (existentesEnDestino >= capacidad)
                        throw new InvalidOperationException(
                            $"Capacidad del sector ({capacidad}) excedida. No puedes asignar más slots al sector {sectorDestino.Name}."
                        );
                }

                // Mapear cambios sobre la entidad actual (uso _mapper si quieres; aquí lo hago manual)
                actual.Name = dto.Name;
                actual.SectorsId = dto.SectorsId;
                actual.IsAvailable = dto.IsAvailable;
                actual.Asset = dto.Asset.GetValueOrDefault(true);
                actual.IsDeleted = dto.IsDeleted.GetValueOrDefault(false);
                // mantener otras propiedades que no quieres que cambien

                await _data.Update(actual);

                // obtener la entidad persistida si quieres devolverla completa
                var persisted = await _data.GetById(dto.Id);
                return new SlotsDto
                {
                    Id = persisted.Id,
                    Name = persisted.Name,
                    SectorsId = persisted.SectorsId,
                    IsAvailable = persisted.IsAvailable,
                    Asset = persisted.Asset,
                    IsDeleted = persisted.IsDeleted
                };
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
                Console.WriteLine("Exception en SlotsBusiness.Update:");
                Console.WriteLine(ex.ToString());

                throw new BusinessException("Error al actualizar el registro del slot.", ex);
            }
        }

        public Task<OccupancyDto> GetOccupancyGlobalAsync()
       => _data.GetOccupancyGlobalAsync();


        public async Task<IEnumerable<SlotsDto>> GetAllByParkingIdAsync(int parkingId)
        {
            try
            {
                if (parkingId <= 0)
                    throw new ArgumentException("El id del parqueadero no es válido.");

                var slots = await _data.GetAllByParkingIdAsync(parkingId);

                if (slots == null || !slots.Any())
                    throw new InvalidOperationException("No se encontraron slots para el parqueadero especificado.");

                return slots;
            }
            catch (InvalidOperationException) { throw; }
            catch (ArgumentException) { throw; }
            catch (Exception ex)
            {
                throw new BusinessException("Error al obtener los slots por parqueadero.", ex);
            }
        }


        public async Task<Slots> AssignAvailableSlotAsync(int typeVehicleId, int parkingId)
        {
            List<Sectors> validSectors = await _sectorsBusiness.GetSectorsByVehicleTypeAsync(typeVehicleId, parkingId);

            List<Slots> availableSlots = new List<Slots>();

            foreach (Sectors sector in validSectors)
            {
                foreach (Slots slot in sector.Slots)
                {
                    bool occupied = await _registeredVehicleData.AnyActiveRegisteredVehicleInSlotAsync(slot.Id);
                    if (!occupied && slot.IsAvailable)
                        availableSlots.Add(slot);
                }
            }

            if (availableSlots.Count == 0)
                throw new BusinessException("No hay slots disponibles para este tipo de vehículo.");

            Slots assignedSlot = availableSlots[new Random().Next(availableSlots.Count)];
            return assignedSlot;
        }

    }
}
