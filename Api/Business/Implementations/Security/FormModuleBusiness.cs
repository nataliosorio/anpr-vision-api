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

namespace Business.Implementations.Security
{
    public class FormModuleBusiness : RepositoryBusiness<FormModule, FormModuleDto>, IFormModuleBusiness
    {
        private readonly IPersonParkignData _data;
        private readonly IMapper _mapper;
        private readonly ILogger<FormModuleBusiness> _logger;
        public FormModuleBusiness(IPersonParkignData data, IMapper mapper, ILogger<FormModuleBusiness> logger)
            : base(data, mapper)
        {
            _data = data;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<FormModuleDto>> GetAllJoinAsync()
        {
            var entities = await _data.GetAllJoinAsync();
            return _mapper.Map<IEnumerable<FormModuleDto>>(entities);
        }


        // SAVE
        public override async Task<FormModuleDto> Save(FormModuleDto dto)
        {
            try
            {
                if (dto == null) throw new ArgumentException("Datos inválidos.");
                if (dto.FormId <= 0) throw new ArgumentException("FormId inválido.");
                if (dto.ModuleId <= 0) throw new ArgumentException("ModuleId inválido.");

                // Comprobación de duplicado: incluir TODAS las filas (también IsDeleted = true)
                bool exists = false;
                try
                {
                    exists = await _data.ExistsAsync(r =>
                        r.FormId == dto.FormId &&
                        r.ModuleId == dto.ModuleId
                    );
                }
                catch (Exception exExists)
                {
                    _logger?.LogWarning(exExists, "ExistsAsync falló en Save, usando GetAll() como fallback.");
                    var all = await _data.GetAll() ?? Enumerable.Empty<FormModule>();
                    exists = all.Any(r =>
                        r.FormId == dto.FormId && r.ModuleId == dto.ModuleId
                    );
                }

                if (exists)
                    throw new ArgumentException("Ya existe un registro con esa misma combinación Formulario-Modulo.");

                dto.Asset = dto.Asset ?? true;
                dto.IsDeleted = dto.IsDeleted ?? false;

                var entity = _mapper.Map<FormModule>(dto);
                var saved = await _data.Save(entity);

                return _mapper.Map<FormModuleDto>(saved);
            }
            catch (ArgumentException) { throw; }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error al registrar FormModule");
                throw new BusinessException("Error al registrar FormModule.", ex);
            }
        }

        // UPDATE
        public override async Task Update(FormModuleDto dto)
        {
            try
            {
                if (dto == null) throw new ArgumentException("Datos inválidos.");
                if (dto.Id <= 0) throw new ArgumentException("Id inválido.");
                if (dto.FormId <= 0) throw new ArgumentException("FormId inválido.");
                if (dto.ModuleId <= 0) throw new ArgumentException("ModuleId inválido.");

                var current = await _data.GetById(dto.Id);
                if (current == null)
                    throw new InvalidOperationException($"No existe el registro con Id {dto.Id}.");

                // Si no cambia la combinación ni otros campos relevantes -> salir
                bool pairChanged = current.FormId != dto.FormId || current.ModuleId != dto.ModuleId;
                bool otherChanged =
                    dto.IsDeleted != null && dto.IsDeleted != current.IsDeleted ||
                    dto.Asset != null && dto.Asset.Value != current.Asset;
                if (!pairChanged && !otherChanged)
                    return;

                // Comprobar duplicado entre todas las filas (incluye IsDeleted = true). Excluir el propio Id.
                bool existsOther = false;
                try
                {
                    existsOther = await _data.ExistsAsync(r =>
                        r.Id != dto.Id &&
                        r.FormId == dto.FormId &&
                        r.ModuleId == dto.ModuleId
                    );
                }
                catch (Exception exExists)
                {
                    _logger?.LogWarning(exExists, "ExistsAsync falló en Update; usando GetAll() como fallback.");
                    var all = await _data.GetAll() ?? Enumerable.Empty<FormModule>();
                    existsOther = all.Any(r =>
                        r.Id != dto.Id &&
                        r.FormId == dto.FormId &&
                        r.ModuleId == dto.ModuleId
                    );
                }

                if (existsOther)
                    throw new ArgumentException("Ya existe otro registro con esa misma combinación Formulario-Modulo.");

                // Mapear y actualizar
                current.FormId = dto.FormId;
                current.ModuleId = dto.ModuleId;
                if (dto.IsDeleted != null) current.IsDeleted = dto.IsDeleted;
                if (dto.Asset != null) current.Asset = dto.Asset.Value;

                try
                {
                    await _data.Update(current);
                }
                catch (Microsoft.EntityFrameworkCore.DbUpdateException dbEx)
                {
                    _logger?.LogError(dbEx, "DbUpdateException en FormModule.Update - posible violación de unicidad");
                    throw new ArgumentException("No fue posible actualizar: ya existe otro registro con la misma combinación Formulario-Modulo.");
                }
            }
            catch (ArgumentException) { throw; }
            catch (InvalidOperationException) { throw; }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error al actualizar FormModule");
                throw new BusinessException("Error al actualizar FormModule.", ex);
            }
        }


    }
}
