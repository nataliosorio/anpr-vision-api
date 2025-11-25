using AutoMapper;
using Business.Interfaces.Security;
using Data.Interfaces.Security;
using Entity.Dtos.Security;
using Entity.Models.Security;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities.Exceptions;
using Utilities.Helpers.Validators;

namespace Business.Implementations.Security
{
    public class ModuleBusiness : RepositoryBusiness<Module, ModuleDto>, IModuleBusiness
    {
        private readonly IModuleData _data;
        private readonly IMapper _mapper;
        private readonly ILogger<ModuleBusiness> _logger;
        public ModuleBusiness(IModuleData data, IMapper mapper, ILogger<ModuleBusiness> logger)
            : base(data, mapper)
        {
            _data = data;
            _mapper = mapper;
            _logger = logger;
        }

        public override async Task<ModuleDto> Save(ModuleDto dto)
        {
            try
            {
                Validations.ValidateDto(dto, "Name");

                dto.Name = dto.Name?.Trim();

                if (string.IsNullOrWhiteSpace(dto.Name))
                    throw new ArgumentException("El nombre es obligatorio.");
                if (dto.Name.Length < 2)
                    throw new ArgumentException("El nombre debe tener al menos 2 caracteres.");
                if (dto.Name.Length > 100)
                    throw new ArgumentException("El nombre no puede superar los 100 caracteres.");

                // Defaults tri-estado
                if (dto.Asset == null) dto.Asset = true;
                if (dto.IsDeleted == null) dto.IsDeleted = false;

                var nameNorm = dto.Name.ToUpperInvariant();

                bool exists = false;
                try
                {
                    exists = await _data.ExistsAsync(m =>
                        m.Name != null &&
                        m.Name.ToUpper() == nameNorm &&
                        (m.IsDeleted == null || m.IsDeleted == false)
                    );
                }
                catch
                {
                    // Fallback si ExistsAsync lanza: comprobar en memoria (menos eficiente)
                    var all = await _data.GetAll() ?? Enumerable.Empty<Module>();
                    exists = all.Any(m =>
                        !string.IsNullOrWhiteSpace(m.Name) &&
                        string.Equals(m.Name.Trim(), dto.Name, StringComparison.OrdinalIgnoreCase) &&
                        !(m.IsDeleted ?? false)
                    );
                }

                if (exists)
                    throw new ArgumentException($"Ya existe un módulo con el nombre '{dto.Name}'.");

                dto.Asset = dto.Asset.GetValueOrDefault(true);
                dto.IsDeleted = dto.IsDeleted.GetValueOrDefault(false);

                var entity = _mapper.Map<Module>(dto);
                entity = await _data.Save(entity);

                return _mapper.Map<ModuleDto>(entity);
            }
            catch (ArgumentException) { throw; }
            catch (Exception ex)
            {
                throw new BusinessException("Error al crear el módulo.", ex);
            }
        }

        public override async Task Update(ModuleDto dto)
        {
            try
            {
                Validations.ValidateDto(dto, "Id", "Name");

                if (dto.Id <= 0)
                    throw new ArgumentException("El Id debe ser mayor que 0.");

                var current = await _data.GetById(dto.Id);
                if (current == null)
                    throw new InvalidOperationException($"No existe un módulo con Id {dto.Id}.");
            

                dto.Name = dto.Name?.Trim();

                if (string.IsNullOrWhiteSpace(dto.Name))
                    throw new ArgumentException("El nombre es obligatorio.");
                if (dto.Name.Length < 2)
                    throw new ArgumentException("El nombre debe tener al menos 2 caracteres.");
                if (dto.Name.Length > 100)
                    throw new ArgumentException("El nombre no puede superar los 100 caracteres.");

                // Detectar si realmente hay cambios (evita update innecesario)
                var nameNorm = (dto.Name ?? string.Empty).Trim();
                var currentNameNorm = (current.Name ?? string.Empty).Trim();
                bool nameChanged = !string.Equals(currentNameNorm, nameNorm, StringComparison.OrdinalIgnoreCase);

                bool otherChanges =
                    !string.Equals((current.Description ?? string.Empty).Trim(), (dto.Description ?? string.Empty).Trim(), StringComparison.Ordinal) ||
                    dto.Asset != null && dto.Asset.Value != current.Asset ||
                    dto.IsDeleted != null && dto.IsDeleted.Value != (current.IsDeleted ?? false);

                if (!nameChanged && !otherChanges)
                    return; // nada que actualizar

                // Comprobar conflicto usando ExistsAsynca (pasa el Id actual para excluirlo)
                bool conflict = false;
                try
                {
                    conflict = await _data.ExistsAsynca("Name", dto.Name, dto.Id);
                }
                catch
                {
                    // fallback: comprobar en memoria
                    var all = await _data.GetAll() ?? Enumerable.Empty<Module>();
                    conflict = all.Any(m =>
                        m.Id != dto.Id &&
                        !string.IsNullOrWhiteSpace(m.Name) &&
                        string.Equals(m.Name.Trim(), dto.Name, StringComparison.OrdinalIgnoreCase) &&
                        !(m.IsDeleted ?? false)
                    );
                }

                if (conflict)
                    throw new ArgumentException($"Ya existe otro módulo con el nombre '{dto.Name}'.");

                // Defaults null-safe
                if (dto.Asset == null) dto.Asset = current.Asset;
                if (dto.IsDeleted == null) dto.IsDeleted = current.IsDeleted;

                // MAPEAR sobre la entidad trackeada para evitar problemas de tracking
                _mapper.Map(dto, current);

                await _data.Update(current);
            }
            catch (ArgumentException) { throw; }
            catch (InvalidOperationException) { throw; }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error al actualizar el módulo");
                throw new BusinessException("Error al actualizar el módulo.", ex);
            }
        }

    }
}
