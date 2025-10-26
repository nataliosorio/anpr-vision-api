using AutoMapper;
using Business.Interfaces.Parameter;
using Data.Interfaces.Parameter;
using Entity.Dtos.Parameter;
using Entity.Models.Parameter;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities.Exceptions;
using Utilities.Helpers.Validators;

namespace Business.Implementations.Parameter
{
 
    public class ParkingCategoryBusiness : RepositoryBusiness<ParkingCategory, ParkingCategoryDto>, IParkingCategoryBusiness
    {
        private readonly IParkingCategoryData _data;
        private readonly IMapper _mapper;
        private readonly ILogger<ParkingCategoryBusiness> _logger;
        public ParkingCategoryBusiness(IParkingCategoryData data, IMapper mapper, ILogger<ParkingCategoryBusiness> logger)
            : base(data, mapper)
        {
            _data = data;
            _mapper = mapper;
            _logger = logger;
        }

        // ParkingCategoryBusiness.cs (Save)
        public override async Task<ParkingCategoryDto> Save(ParkingCategoryDto dto)
        {
            try
            {
                Validations.ValidateDto(dto, "Code", "Name");

                dto.Name = dto.Name?.Trim();
                dto.Code = dto.Code?.Trim().ToUpperInvariant();

                if (string.IsNullOrWhiteSpace(dto.Name))
                    throw new ArgumentException("El nombre es obligatorio.");
                if (dto.Name.Length > 50)
                    throw new ArgumentException("El nombre no puede contener más de 50 caracteres.");
                if (string.IsNullOrWhiteSpace(dto.Code))
                    throw new ArgumentException("El código es obligatorio.");

                // Defaults
                dto.Asset ??= true;
                dto.IsDeleted ??= false;

                // FIABLE: comprobar en memoria (determinístico)
                var all = await _data.GetAll() ?? Enumerable.Empty<ParkingCategory>();

                var nameDup = all.Any(pc =>
                    !string.IsNullOrWhiteSpace(pc.Name) &&
                    string.Equals(pc.Name.Trim(), dto.Name, StringComparison.OrdinalIgnoreCase) &&
                    !(pc.IsDeleted ?? false)
                );

                var codeDup = all.Any(pc =>
                    !string.IsNullOrWhiteSpace(pc.Code) &&
                    string.Equals(pc.Code.Trim(), dto.Code, StringComparison.OrdinalIgnoreCase) &&
                    !(pc.IsDeleted ?? false)
                );

                _logger?.LogDebug("Save checks: nameDup={NameDup}, codeDup={CodeDup}", nameDup, codeDup);

                if (nameDup)
                    throw new ArgumentException($"Ya existe una categoría de parqueadero con el nombre '{dto.Name}'.");
                if (codeDup)
                    throw new ArgumentException($"Ya existe una categoría de parqueadero con el código '{dto.Code}'.");

                var entity = _mapper.Map<ParkingCategory>(dto);
                entity.Id = 0;
                entity.Asset = dto.Asset.GetValueOrDefault(true);
                entity.IsDeleted = dto.IsDeleted.GetValueOrDefault(false);

                try
                {
                    entity = await _data.Save(entity);
                }
                catch (Microsoft.EntityFrameworkCore.DbUpdateException dbEx)
                {
                    // Capturar violación de índice único en BD si existe
                    _logger?.LogError(dbEx, "DbUpdateException en Save ParkingCategory");
                    throw new ArgumentException("No fue posible crear la categoría porque ya existe un registro con el mismo nombre o código.");
                }

                return _mapper.Map<ParkingCategoryDto>(entity);
            }
            catch (ArgumentException) { throw; }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error al crear ParkingCategory");
                throw new BusinessException("Error al crear el registro.", ex);
            }
        }


        // ParkingCategoryBusiness.cs (Update)
        public override async Task Update(ParkingCategoryDto dto)
        {
            try
            {
                Validations.ValidateDto(dto, "Id", "Code", "Name");

                if (dto.Id <= 0)
                    throw new ArgumentException("No ha seleccionado ninguna categoría.");

                var current = await _data.GetById(dto.Id);
                if (current == null)
                    throw new InvalidOperationException("Seleccione una categoría de parqueadero válida.");
                if (!current.Asset)
                    throw new InvalidOperationException("No se puede actualizar una categoría deshabilitada.");

                dto.Name = dto.Name?.Trim();
                dto.Code = dto.Code?.Trim().ToUpperInvariant();

                if (string.IsNullOrWhiteSpace(dto.Name))
                    throw new ArgumentException("El nombre es obligatorio.");
                if (dto.Name.Length > 50)
                    throw new ArgumentException("El nombre no puede contener más de 50 caracteres.");
                if (string.IsNullOrWhiteSpace(dto.Code))
                    throw new ArgumentException("El código es obligatorio.");

                // Comprobación FIABLE en memoria
                var all = await _data.GetAll() ?? Enumerable.Empty<ParkingCategory>();

                var nameExistsOther = all.Any(pc =>
                    pc.Id != dto.Id &&
                    !string.IsNullOrWhiteSpace(pc.Name) &&
                    string.Equals(pc.Name.Trim(), dto.Name, StringComparison.OrdinalIgnoreCase) &&
                    !(pc.IsDeleted ?? false)
                );

                var codeExistsOther = all.Any(pc =>
                    pc.Id != dto.Id &&
                    !string.IsNullOrWhiteSpace(pc.Code) &&
                    string.Equals(pc.Code.Trim(), dto.Code, StringComparison.OrdinalIgnoreCase) &&
                    !(pc.IsDeleted ?? false)
                );

                _logger?.LogDebug("Update checks: nameExistsOther={NameExists}, codeExistsOther={CodeExists}", nameExistsOther, codeExistsOther);

                if (nameExistsOther)
                    throw new ArgumentException($"Ya existe otra categoría de parqueadero con el nombre '{dto.Name}'.");
                if (codeExistsOther)
                    throw new ArgumentException($"Ya existe otra categoría de parqueadero con el código '{dto.Code}'.");

                dto.Asset ??= current.Asset;
                dto.IsDeleted ??= current.IsDeleted;

                _mapper.Map(dto, current);

                try
                {
                    await _data.Update(current);
                }
                catch (Microsoft.EntityFrameworkCore.DbUpdateException dbEx)
                {
                    _logger?.LogError(dbEx, "DbUpdateException en Update ParkingCategory");
                    throw new ArgumentException("No fue posible actualizar la categoría porque existe otra con el mismo nombre o código.");
                }
            }
            catch (ArgumentException) { throw; }
            catch (InvalidOperationException) { throw; }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error al actualizar ParkingCategory");
                throw new BusinessException("Error al actualizar el registro.", ex);
            }
        }





    }
}
