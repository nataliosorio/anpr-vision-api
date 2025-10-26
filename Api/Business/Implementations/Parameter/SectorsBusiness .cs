using AutoMapper;
using Business.Interfaces.Parameter;
using Data.Interfaces.Parameter;
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
   
    public class SectorsBusiness : RepositoryBusiness<Sectors, SectorsDto>, ISectorsBusiness
    {
        private readonly ISectorsData _data;
        private readonly IMapper _mapper;
        public SectorsBusiness(ISectorsData data, IMapper mapper)
            : base(data, mapper)
        {
            _data = data;
            _mapper = mapper;
        }

        public async Task<IEnumerable<SectorsDto>> GetAllJoinAsync()
        {
            try
            {
                IEnumerable<SectorsDto> entities = await _data.GetAllJoinAsync();
                if (!entities.Any()) throw new InvalidOperationException("No se encontraron sectores.");
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
                throw new Exception("Error al obtener los sectores.", ex);
            }
        }

        public async Task<IEnumerable<SectorsDto>> GetAllByZoneId(int zoneId)
        {
            try
            {
                if (zoneId < 1) throw new ArgumentException("El id de la zona es invalido.");
                IEnumerable<Sectors> entities = await _data.GetAllByZoneId(zoneId);
                if (!entities.Any()) throw new InvalidOperationException("No se encontraron sectores para la zona seleccionada.");
                return _mapper.Map<IEnumerable<SectorsDto>>(entities);
            }
            catch(InvalidOperationException invEx)
            {
                throw new InvalidOperationException("error: ", invEx);
            }
            catch (ArgumentException argEx)
            {
                throw new ArgumentException("error: ", argEx);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener los sectores de la zona.", ex);
            }
        }

        // Pseudocódigo detallado:
        // 1. El método Save intenta validar que no exista un sector duplicado usando _data.ExistsAsync<Sectors>.
        // 2. Sin embargo, ISectorsData no tiene un método ExistsAsync definido.
        // 3. Solución: Reemplazar la llamada a ExistsAsync por una consulta manual usando GetAllByZoneId y filtrando por TypeVehicleId y Asset.
        // 4. Obtener todos los sectores de la zona, filtrar por TypeVehicleId y Asset == true, y verificar si existe alguno.

        public override async Task<SectorsDto> Save(SectorsDto dto)
        {
            try
            {
                // Validaciones básicas (lanza ArgumentException si falta algún campo)
                Validations.ValidateDto(dto, "Name", "Capacity", "ZonesId");

                dto.Name = dto.Name?.Trim();

                if (string.IsNullOrWhiteSpace(dto.Name))
                    throw new ArgumentException("El campo Name es obligatorio.");
                if (dto.Name.Length < 2)
                    throw new ArgumentException("El nombre debe tener al menos 2 caracteres.");
                if (dto.Name.Length > 100)
                    throw new ArgumentException("El nombre no puede superar los 100 caracteres.");

                if (dto.Capacity <= 0)
                    throw new ArgumentException("El campo Capacity debe ser mayor a 0.");
                if (dto.Capacity > 5000)
                    throw new ArgumentException("El campo Capacity no puede superar los 5000 espacios.");

                if (dto.ZonesId <= 0)
                    throw new ArgumentException("El campo ZonesId debe ser mayor a 0.");

                // 🔍 Duplicado por NOMBRE en la misma zona (ignora mayúsculas, null-safe)
                var sectoresMismaZona = await _data.GetAllByZoneId(dto.ZonesId) ?? Enumerable.Empty<Sectors>();
                var nombreDuplicado = sectoresMismaZona.Any(s =>
                    !string.IsNullOrWhiteSpace(s.Name) &&
                    string.Equals(s.Name.Trim(), dto.Name, StringComparison.OrdinalIgnoreCase) &&
                    !(s.IsDeleted ?? false)   // cuenta solo los NO eliminados
                );
                if (nombreDuplicado)
                    throw new ArgumentException($"Ya existe un sector con el nombre '{dto.Name}' en esta zona.");

                // asignaciones por defecto para evitar tri-estado
                //if (dto.Asset == null) dto.Asset = true;
                //if (dto.IsDeleted == null) dto.IsDeleted = false;

                dto.Asset = true;
                dto.IsDeleted = false;


                // Mapear manualmente para evitar problemas con relaciones
                var entity = new Sectors
                {
                    Name = dto.Name,
                    Capacity = dto.Capacity,
                    ZonesId = dto.ZonesId,
                    TypeVehicleId = dto.TypeVehicleId,
                    Asset = dto.Asset.GetValueOrDefault(true),
                    IsDeleted = dto.IsDeleted.GetValueOrDefault(false)
                    // agrega aquí otras propiedades simples si las necesitas
                };

                // Guardar
                entity = await _data.Save(entity); // si tu Save devuelve BaseModel, ajusta el cast

                // Devolver DTO con los valores persistidos
                return new SectorsDto
                {
                    Id = entity.Id,
                    Name = entity.Name,
                    Capacity = entity.Capacity,
                    ZonesId = entity.ZonesId,
                    TypeVehicleId = entity.TypeVehicleId,
                    Zones = null,
                    TypeVehicle = null,
                    Asset = entity.Asset,
                    IsDeleted = entity.IsDeleted
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
                // opcional: Console.WriteLine(ex.ToString()); para debugging local
                throw new BusinessException("Error al crear el registro del sector.", ex);
            }
        }

        // UPDATE (excluye propio Id al validar duplicado)
        public override async Task Update(SectorsDto dto)
        {
            try
            {
                Validations.ValidateDto(dto, "Id", "Name", "Capacity", "ZonesId");

                if (dto.Id <= 0)
                    throw new ArgumentException("El campo Id debe ser mayor a 0.");

                dto.Name = dto.Name?.Trim();

                var sectorExistente = await _data.GetById(dto.Id);
                if (sectorExistente == null)
                    throw new InvalidOperationException($"No existe un sector con Id {dto.Id}.");

                if (string.IsNullOrWhiteSpace(dto.Name))
                    throw new ArgumentException("El campo Name es obligatorio.");
                if (dto.Name.Length < 2)
                    throw new ArgumentException("El nombre debe tener al menos 2 caracteres.");
                if (dto.Name.Length > 100)
                    throw new ArgumentException("El nombre no puede superar los 100 caracteres.");

                if (dto.Capacity <= 0)
                    throw new ArgumentException("El campo Capacity debe ser mayor a 0.");
                if (dto.Capacity > 5000)
                    throw new ArgumentException("El campo Capacity no puede superar los 5000 espacios.");

                if (dto.ZonesId <= 0)
                    throw new ArgumentException("El campo ZonesId debe ser mayor a 0.");

                if (!sectorExistente.Asset)
                    throw new InvalidOperationException("No se puede actualizar un sector deshabilitado.");

                // Duplicado por NOMBRE en la misma zona, excluyendo este mismo Id (null-safe)
                var sectoresMismaZona = await _data.GetAllByZoneId(dto.ZonesId) ?? Enumerable.Empty<Sectors>();
                var nombreDuplicadoOtro = sectoresMismaZona.Any(s =>
                    s.Id != dto.Id &&
                    !string.IsNullOrWhiteSpace(s.Name) &&
                    string.Equals(s.Name.Trim(), dto.Name, StringComparison.OrdinalIgnoreCase) &&
                    !(s.IsDeleted ?? false)
                );
                if (nombreDuplicadoOtro)
                    throw new ArgumentException($"Ya existe otro sector con el nombre '{dto.Name}' en esta zona.");

                // Defaults null-safe
                if (dto.Asset == null) dto.Asset = true;
                if (dto.IsDeleted == null) dto.IsDeleted = false;

                // Mapear sobre la entidad actual (mantener lo que no quieres cambiar)
                sectorExistente.Name = dto.Name;
                sectorExistente.Capacity = dto.Capacity;
                sectorExistente.ZonesId = dto.ZonesId;
                sectorExistente.TypeVehicleId = dto.TypeVehicleId;
                sectorExistente.Asset = dto.Asset.GetValueOrDefault(true);
                // conservar IsDeleted según lo que venga en dto (o mantener current.IsDeleted según tu lógica)

                await _data.Update(sectorExistente);
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
                // opcional: Console.WriteLine(ex.ToString());
                throw new BusinessException("Error al actualizar el registro del sector.", ex);
            }
        }
        public async Task<List<Sectors>> GetSectorsByVehicleTypeAsync(int vehicleTypeId , int parkingId)
        {
            List<Sectors> sectors = await _data.GetSectorsByVehicleTypeAsync(vehicleTypeId , parkingId);
            return sectors;
        }
    }
}
