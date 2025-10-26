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
  
    public class MemberShipTypeBusiness : RepositoryBusiness<MemberShipType, MemberShipTypeDto>, IMemberShipTypeBusiness
    {
        private readonly IMemberShipTypeData _data;
        private readonly IMapper _mapper;
        public MemberShipTypeBusiness(IMemberShipTypeData data, IMapper mapper)
            : base(data, mapper)
        {
            _data = data;
            _mapper = mapper;
        }
        public override async Task<MemberShipTypeDto> Save(MemberShipTypeDto dto)
        {
            try
            {
                // Asegúrate de validar Name además de Description si lo usas
                Validations.ValidateDto(dto, "Name", "Description", "PriceBase", "DurationDaysBase");

                dto.Name = dto.Name?.Trim();
                dto.Description = dto.Description?.Trim();

                if (string.IsNullOrWhiteSpace(dto.Name))
                    throw new ArgumentException("El campo 'Name' es obligatorio.");
                if (dto.Name.Length < 2)
                    throw new ArgumentException("El campo 'Name' debe tener al menos 2 caracteres.");
                if (dto.Description == null || dto.Description.Length < 3)
                    throw new ArgumentException("El campo 'Description' debe tener al menos 3 caracteres.");

                if (dto.PriceBase <= 0)
                    throw new ArgumentException("El campo PriceBase debe ser mayor que 0.");

                if (dto.DurationDaysBase <= 0)
                    throw new ArgumentException("El campo DurationDaysBase debe ser mayor que 0.");

                // Defaults
                dto.Asset ??= true;
                dto.IsDeleted ??= false;

                bool exists = false;
                try
                {
                    // Usa la versión genérica por nombre de campo si la tienes
                    if (_data != null)
                        exists = await _data.ExistsAsynca("Name", dto.Name!, null);
                }
                catch
                {
                    // fallback: comprobar en memoria para ser determinístico
                    var all = await _data.GetAll() ?? Enumerable.Empty<MemberShipType>();
                    exists = all.Any(x =>
                        !string.IsNullOrWhiteSpace(x.Name) &&
                        string.Equals(x.Name.Trim(), dto.Name, StringComparison.OrdinalIgnoreCase) &&
                        !(x.IsDeleted ?? false)
                    );
                }

                if (exists)
                    throw new InvalidOperationException("Ya existe un tipo de membresía activo con el mismo nombre.");

                dto.Asset = dto.Asset.GetValueOrDefault(true);
                dto.IsDeleted = dto.IsDeleted.GetValueOrDefault(false);

                var entity = _mapper.Map<MemberShipType>(dto);
                entity.Id = 0; // forzar insert si vienen ids por error
                entity = await _data.Save(entity);

                return _mapper.Map<MemberShipTypeDto>(entity);
            }
            catch (InvalidOperationException) { throw; }
            catch (ArgumentException) { throw; }
            catch (Exception ex)
            {
                throw new BusinessException("Error al registrar el tipo de membresía.", ex);
            }
        }

        // Pseudocódigo detallado para solucionar el error:
        // 1. Reemplazar la llamada a _data.GetByIdAsync<MemberShipType>(dto.Id) por _data.GetById(dto.Id), ya que GetById está definido en IRepositoryData<T>.
        // 2. Reemplazar la llamada a _data.ExistsAsync<MemberShipType>(...) por una consulta manual usando _data.GetAll() y LINQ, ya que ExistsAsync no está definido.
        // 3. Ajustar el método Update para usar estas alternativas.

        public override async Task Update(MemberShipTypeDto dto)
        {
            try
            {
                Validations.ValidateDto(dto, "Id", "Name", "Description", "PriceBase", "DurationDaysBase");

                if (dto.Id <= 0)
                    throw new ArgumentException("El campo Id debe ser mayor que 0.");

                var current = await _data.GetById(dto.Id);
                if (current == null)
                    throw new InvalidOperationException($"No existe un tipo de membresía con Id {dto.Id}.");
                if (!current.Asset)
                    throw new InvalidOperationException("No se puede actualizar un tipo de membresía deshabilitado.");

                dto.Name = dto.Name?.Trim();
                dto.Description = dto.Description?.Trim();

                if (string.IsNullOrWhiteSpace(dto.Name))
                    throw new ArgumentException("El campo 'Name' es obligatorio.");
                if (dto.Name.Length < 2)
                    throw new ArgumentException("El campo 'Name' debe tener al menos 2 caracteres.");
                if (dto.Description == null || dto.Description.Length < 3)
                    throw new ArgumentException("El campo 'Description' debe tener al menos 3 caracteres.");

                if (dto.PriceBase <= 0)
                    throw new ArgumentException("El campo PriceBase debe ser mayor que 0.");

                if (dto.DurationDaysBase <= 0)
                    throw new ArgumentException("El campo DurationDaysBase debe ser mayor que 0.");

                // Si no cambia el Name ni otros campos relevantes, salir (evita actualizaciones inútiles)
                bool nameChanged = !string.Equals(current.Name?.Trim() ?? string.Empty, dto.Name, StringComparison.OrdinalIgnoreCase);
                bool otherChanged =
                    !string.Equals(current.Description?.Trim() ?? string.Empty, dto.Description ?? string.Empty, StringComparison.Ordinal) ||
                    dto.PriceBase != current.PriceBase ||
                    dto.DurationDaysBase != current.DurationDaysBase ||
                    dto.Asset != null && dto.Asset.Value != current.Asset ||
                    dto.IsDeleted != null && dto.IsDeleted.Value != (current.IsDeleted ?? false);

                if (!nameChanged && !otherChanged)
                    return;

                // Verificar duplicado en otros registros (excluir el propio Id)
                bool existsOther = false;
                try
                {
                    existsOther = await _data.ExistsAsynca("Name", dto.Name!, dto.Id);
                }
                catch
                {
                    // fallback a GetAll
                    var all = await _data.GetAll() ?? Enumerable.Empty<MemberShipType>();
                    existsOther = all.Any(x =>
                        x.Id != dto.Id &&
                        !string.IsNullOrWhiteSpace(x.Name) &&
                        string.Equals(x.Name.Trim(), dto.Name, StringComparison.OrdinalIgnoreCase) &&
                        !(x.IsDeleted ?? false)
                    );
                }

                if (existsOther)
                    throw new InvalidOperationException("Ya existe otro tipo de membresía activo con el mismo nombre.");

                // Defaults null-safe
                if (dto.Asset == null) dto.Asset = current.Asset;
                if (dto.IsDeleted == null) dto.IsDeleted = current.IsDeleted;

                // Mapear sobre la entidad trackeada (evita error EF Core tracking)
                _mapper.Map(dto, current);

                await _data.Update(current);
            }
            catch (InvalidOperationException) { throw; }
            catch (ArgumentException) { throw; }
            catch (Exception ex)
            {
                throw new BusinessException("Error al actualizar el tipo de membresía.", ex);
            }
        }

    }
}
