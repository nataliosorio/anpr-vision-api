using AutoMapper;
using Business.Interfaces.Security;
using Data.Interfaces.Security;
using Entity.Dtos.Operational;
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
    public class RolBusiness : RepositoryBusiness<Rol, RolDto>, IRolBusiness
    {
        private readonly IRolData _data;
        private readonly IMapper _mapper;
        private readonly ILogger<RolBusiness> _logger;
        public RolBusiness(IRolData data, IMapper mapper, ILogger<RolBusiness> logger)
            : base(data, mapper)
        {
            _data = data;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<RolDto>> GetAllByParkingAsync()
        {
            try
            {
                var roles = await _data.GetAllByParkingAsync();

                // Mapear los roles a DTO
                return _mapper.Map<IEnumerable<RolDto>>(roles);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener los roles por parking");
                throw new BusinessException("Error al obtener los roles por parking", ex);
            }
        }



        public async Task<RolDto> GetByNameAsync(string name)
        {
            try
            {
                var rolEntity = await _data.GetByNameAsync(name);
                return _mapper.Map<RolDto>(rolEntity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el rol por nombre");
                throw new BusinessException("Error al obtener el rol por nombre", ex);
            }
        }
        public override async Task<RolDto> Save(RolDto dto)
        {
            try
            {
                // Si tienes un helper de validación tipo Validations.ValidateDto, úsalo:
                // Validations.ValidateDto(dto, "Name");

                dto.Name = dto.Name?.Trim();

                if (string.IsNullOrWhiteSpace(dto.Name))
                    throw new ArgumentException("El campo Name es obligatorio.");
                if (dto.Name.Length < 2)
                    throw new ArgumentException("El nombre debe tener al menos 2 caracteres.");
                if (dto.Name.Length > 100)
                    throw new ArgumentException("El nombre no puede superar los 100 caracteres.");

                // Opcionales: si en tu RolDto tienes Asset / IsDeleted como bool? (tri-estado)
                if (dto.Asset == null) dto.Asset = true;
                if (dto.IsDeleted == null) dto.IsDeleted = false;

                // 🔍 Duplicado por NOMBRE (null-safe, ignora mayúsculas/minúsculas), solamente entre no eliminados
                var todosRoles = await _data.GetAll() ?? Enumerable.Empty<Rol>();
                var nombreDuplicado = todosRoles.Any(r =>
                    !string.IsNullOrWhiteSpace(r.Name) &&
                    string.Equals(r.Name.Trim(), dto.Name, StringComparison.OrdinalIgnoreCase) &&
                    !(r.IsDeleted ?? false)   // contar solo los NO eliminados
                );
                if (nombreDuplicado)
                    throw new ArgumentException($"Ya existe un rol con el nombre '{dto.Name}'.");

                // Mapear manualmente para evitar sorpresas con AutoMapper/relaciones
                var entity = new Rol
                {
                    Name = dto.Name,
                    Description = dto.Description,
                    Asset = dto.Asset.GetValueOrDefault(true),
                    IsDeleted = dto.IsDeleted.GetValueOrDefault(false)
                    // agrega otras propiedades simples si aplica
                };

                // Guardar
                entity = await _data.Save(entity);

                // Devolver DTO persistido (puedes usar mapper si prefieres)
                return new RolDto
                {
                    Id = entity.Id,
                    Name = entity.Name,
                    Description = entity.Description,
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
                // opcional: _logger.LogError(ex, "Error al crear el rol");
                throw new BusinessException("Error al crear el registro del rol.", ex);
            }
        }

        public override async Task Update(RolDto dto)
        {
            try
            {
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
                var current = await _data.GetById(dto.Id);
                if (current == null)
                    throw new InvalidOperationException($"No existe un rol con Id {dto.Id}.");

              

                // Intentar obtener por nombre de forma segura (GetByNameAsync debería devolver null si no encuentra)
                Rol? existing = null;
                try
                {
                    existing = await _data.GetByNameAsync(dto.Name);
                }
                catch (Exception getByNameEx)
                {
                    // Si falla la consulta por nombre, registramos el error y haremos fallback a GetAll()
                    _logger.LogWarning(getByNameEx, "GetByNameAsync falló, usando fallback a GetAll()");
                }

                // Fallback: si GetByNameAsync no existe o devolvió null, revisamos con GetAll() (case-insensitive)
                if (existing == null)
                {
                    var all = await _data.GetAll() ?? Enumerable.Empty<Rol>();
                    existing = all.FirstOrDefault(r =>
                        r.Id != dto.Id &&
                        !string.IsNullOrWhiteSpace(r.Name) &&
                        string.Equals(r.Name.Trim(), dto.Name, StringComparison.OrdinalIgnoreCase) &&
                        !(r.IsDeleted ?? false)   // sólo no eliminados
                    );
                }
                else
                {
                    // Si GetByNameAsync devolvió algo, asegurarnos de que no sea el mismo registro ni eliminado
                    if (existing.Id == dto.Id || (existing.IsDeleted ?? false))
                        existing = null;
                }

                if (existing != null)
                    throw new ArgumentException($"Ya existe otro rol con el nombre '{dto.Name}'.");

                // Aplicar cambios sobre la entidad trackeada (evita error de EF Core al tener dos instancias con la misma PK)
                current.Name = dto.Name;
                current.Description = dto.Description;

                if (dto.Asset != null) current.Asset = dto.Asset.Value;
                if (dto.IsDeleted != null) current.IsDeleted = dto.IsDeleted.Value;

                await _data.Update(current);
            }
            catch (ArgumentException) { throw; }
            catch (InvalidOperationException) { throw; }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar el rol");
                throw new BusinessException($"Error al actualizar el rol. {ex.Message}", ex);
            }
        }

    }
}
