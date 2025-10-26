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
    public class FormBusiness : RepositoryBusiness<Form, FormDto>, IFormBusiness
    {
        private readonly IFormData _data;
        private readonly IMapper _mapper;
        public FormBusiness(IFormData data, IMapper mapper)
            : base(data, mapper)
        {
            _mapper = mapper;
            _data = data;
        }


        public override async Task<FormDto> Save(FormDto dto)
        {
            try
            {
                // Validaciones base
                Validations.ValidateDto(dto, "Name");

                dto.Name = dto.Name?.Trim();

                if (string.IsNullOrWhiteSpace(dto.Name))
                    throw new ArgumentException("El campo 'Name' es obligatorio.");
                if (dto.Name.Length < 3)
                    throw new ArgumentException("El nombre debe tener al menos 3 caracteres.");
                if (dto.Name.Length > 100)
                    throw new ArgumentException("El nombre no puede superar los 100 caracteres.");

                // Normalizar una sola vez
                var nameNormalized = dto.Name.ToUpperInvariant();

                // Intentar ExistsAsync primero (más eficiente). Si falla, usar GetAll() fallback.
                bool existsByName = false;
                try
                {
                    existsByName = await _data.ExistsAsync(f =>
                        f.Name != null &&
                        f.Name.ToUpper() == nameNormalized
                    // puedes agregar aquí && (f.IsDeleted == null || f.IsDeleted == false) si quieres ignorar eliminados
                    );
                }
                catch
                {
                    var all = await _data.GetAll() ?? Enumerable.Empty<Form>();
                    existsByName = all.Any(f =>
                        !string.IsNullOrWhiteSpace(f.Name) &&
                        string.Equals(f.Name.Trim(), dto.Name, StringComparison.OrdinalIgnoreCase)
                    // y opcionalmente: && !(f.IsDeleted ?? false)
                    );
                }

                if (existsByName)
                    throw new ArgumentException($"Ya existe un formulario con el nombre '{dto.Name}'.");

                // Defaults
                dto.Asset ??= true;
                dto.IsDeleted ??= false;

                // Mapear, forzar Id = 0 (evitar conflictos si el cliente envía Id)
                var entity = _mapper.Map<Form>(dto);
                entity.Id = 0;
                entity.Asset = dto.Asset.GetValueOrDefault(true);
                entity.IsDeleted = dto.IsDeleted.GetValueOrDefault(false);

                try
                {
                    entity = await _data.Save(entity);
                }
                catch (Microsoft.EntityFrameworkCore.DbUpdateException dbEx)
                {
                    // Protección contra condiciones de carrera: BD puede arrojar violación de unicidad
                    throw new ArgumentException("No fue posible crear el formulario: ya existe otro con el mismo nombre.", dbEx);
                }

                return _mapper.Map<FormDto>(entity);
            }
            catch (ArgumentException) { throw; }
            catch (Exception ex)
            {
                throw new BusinessException("Error al registrar el formulario.", ex);
            }
        }

        public override async Task Update(FormDto dto)
        {
            try
            {
                Validations.ValidateDto(dto, "Id", "Name");

                if (dto.Id <= 0)
                    throw new ArgumentException("El Id debe ser mayor que 0.");

                dto.Name = dto.Name?.Trim();

                if (string.IsNullOrWhiteSpace(dto.Name))
                    throw new ArgumentException("El campo 'Name' es obligatorio.");
                if (dto.Name.Length < 3)
                    throw new ArgumentException("El nombre debe tener al menos 3 caracteres.");
                if (dto.Name.Length > 100)
                    throw new ArgumentException("El nombre no puede superar los 100 caracteres.");

                // Obtener la entidad trackeada
                var current = await _data.GetById(dto.Id);
                if (current == null)
                    throw new InvalidOperationException($"No existe un formulario con Id {dto.Id}.");

                if (!current.Asset)
                    throw new InvalidOperationException("No se puede actualizar un formulario deshabilitado.");

                // Normalizar
                var nameNormalized = dto.Name.ToUpperInvariant();

                // Comprobar duplicado en otros registros (excluir propio Id)
                bool existsOtherByName = false;
                try
                {
                    existsOtherByName = await _data.ExistsAsync(f =>
                        f.Name != null &&
                        f.Name.ToUpper() == nameNormalized &&
                        f.Id != dto.Id
                    // agregar && (f.IsDeleted == null || f.IsDeleted == false) si quieres ignorar eliminados
                    );
                }
                catch
                {
                    var all = await _data.GetAll() ?? Enumerable.Empty<Form>();
                    existsOtherByName = all.Any(f =>
                        f.Id != dto.Id &&
                        !string.IsNullOrWhiteSpace(f.Name) &&
                        string.Equals(f.Name.Trim(), dto.Name, StringComparison.OrdinalIgnoreCase)
                    // && !(f.IsDeleted ?? false)
                    );
                }

                if (existsOtherByName)
                    throw new ArgumentException($"Ya existe otro formulario con el nombre '{dto.Name}'.");

                // Mapear SOBRE LA ENTIDAD TRAQUEADA (evita error de EF Core por dos instancias con la misma PK)
                _mapper.Map(dto, current);

                // Asegurar defaults null-safe
                current.Asset = dto.Asset ?? current.Asset;
                current.IsDeleted = dto.IsDeleted ?? current.IsDeleted;

                try
                {
                    await _data.Update(current);
                }
                catch (Microsoft.EntityFrameworkCore.DbUpdateException dbEx)
                {
                    throw new ArgumentException("No fue posible actualizar el formulario: conflicto de unicidad.", dbEx);
                }
            }
            catch (ArgumentException) { throw; }
            catch (InvalidOperationException) { throw; }
            catch (Exception ex)
            {
                throw new BusinessException("Error al actualizar el formulario.", ex);
            }
        }

    }
}
