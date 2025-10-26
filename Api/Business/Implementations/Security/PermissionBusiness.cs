using AutoMapper;
using Business.Interfaces.Security;
using Data.Interfaces.Security;
using Entity.Dtos.Security;
using Entity.Models.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities.Exceptions;
using Utilities.Helpers.Validators;

namespace Business.Implementations.Security
{
    public class PermissionBusiness : RepositoryBusiness<Permission, PermissionDto>, IPermissionBusiness
    {
        private readonly IPermissionData _data;
        private readonly IMapper _mapper;
        public PermissionBusiness(IPermissionData data, IMapper mapper)
            : base(data, mapper)
        {
            _data = data;
            _mapper = mapper;
        }
        public override async Task<PermissionDto> Save(PermissionDto dto)
        {
            try
            {
                // Validación básica del DTO (lanza ArgumentException si falta Name)
                Validations.ValidateDto(dto, "Name");

                dto.Name = dto.Name?.Trim();
                if (string.IsNullOrWhiteSpace(dto.Name))
                    throw new ArgumentException("El campo 'Name' es obligatorio.");
                if (dto.Name.Length < 2)
                    throw new ArgumentException("El nombre debe tener al menos 2 caracteres.");
                if (dto.Name.Length > 100)
                    throw new ArgumentException("El nombre no puede superar los 100 caracteres.");

                // Defaults tri-estado
                if (dto.Asset == null) dto.Asset = true;
                if (dto.IsDeleted == null) dto.IsDeleted = false;

                // Duplicado (case-insensitive) usando ExistsAsync
                var exists = await _data.ExistsAsync(p =>
                    p.Name.ToLower() == dto.Name.ToLower() &&
                    (p.IsDeleted == null || p.IsDeleted == false)
                );
                if (exists)
                    throw new ArgumentException($"Ya existe un permiso con el nombre '{dto.Name}'.");

                // Mapear y guardar
                var entity = _mapper.Map<Permission>(dto);
                entity = await _data.Save(entity);

                return _mapper.Map<PermissionDto>(entity);
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
                throw new BusinessException("Error al crear el permiso.", ex);
            }
        }

        public override async Task Update(PermissionDto dto)
        {
            try
            {
                // Validar que venga Id y Name
                Validations.ValidateDto(dto, "Id", "Name");

                if (dto.Id <= 0)
                    throw new ArgumentException("El Id debe ser mayor que 0.");

                dto.Name = dto.Name?.Trim();
                if (string.IsNullOrWhiteSpace(dto.Name))
                    throw new ArgumentException("El campo 'Name' es obligatorio.");
                if (dto.Name.Length < 2)
                    throw new ArgumentException("El nombre debe tener al menos 2 caracteres.");
                if (dto.Name.Length > 100)
                    throw new ArgumentException("El nombre no puede superar los 100 caracteres.");

                // Obtener la entidad trackeada
                var current = await _data.GetById(dto.Id)
                              ?? throw new InvalidOperationException($"No existe un permiso con Id {dto.Id}.");

                if (current.Asset == false)
                    throw new InvalidOperationException("No se puede actualizar un permiso deshabilitado.");

                // Duplicado en otros registros (case-insensitive)
                var existsOther = await _data.ExistsAsync(p =>
                    p.Name.ToLower() == dto.Name.ToLower() &&
                    p.Id != dto.Id &&
                    (p.IsDeleted == null || p.IsDeleted == false)
                );
                if (existsOther)
                    throw new ArgumentException($"Ya existe otro permiso con el nombre '{dto.Name}'.");

                // Defaults null-safe: conservar valores actuales si dto no los trae
                if (dto.Asset == null) dto.Asset = current.Asset;
                if (dto.IsDeleted == null) dto.IsDeleted = current.IsDeleted;

                // MAPEAR sobre la entidad trackeada (evita error de EF Core por duplicado en ChangeTracker)
                _mapper.Map(dto, current);

                // Si prefieres no usar AutoMapper, asigna manualmente:
                // current.Name = dto.Name;
                // current.Description = dto.Description;
                // if (dto.Asset != null) current.Asset = dto.Asset.Value;
                // if (dto.IsDeleted != null) current.IsDeleted = dto.IsDeleted.Value;

                await _data.Update(current);
            }
            catch (ArgumentException) { throw; }
            catch (InvalidOperationException) { throw; }
            catch (Exception ex)
            {
                throw new BusinessException("Error al actualizar el permiso.", ex);
            }
        }
    }
}
