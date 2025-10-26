using AutoMapper;
using Business.Interfaces.Parameter;
using Data.Interfaces.Parameter;
using Entity.Dtos.Parameter;
using Entity.Models;
using Entity.Models.Parameter;
using Microsoft.EntityFrameworkCore;
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
  
    public class ParkingBusiness : RepositoryBusiness<Parking, ParkingDto>, IParkingBusiness
    {
        private readonly IParkingData _data;
        private readonly IMapper _mapper;
        public ParkingBusiness(IParkingData data, IMapper mapper)
            : base(data, mapper)
        {
            _data = data;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ParkingDto>> GetAllJoinAsync()
        {
            try
            {
                IEnumerable<ParkingDto> entities = await _data.GetAllJoinAsync();
                if (!entities.Any()) throw new InvalidOperationException("No se encontraron los parqueaderos.");
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
                throw new Exception("Error al obtener los parqueaderos.", ex);
            }
        }

        public override async Task<ParkingDto> Save(ParkingDto dto)
        {
            try
            {
                Validations.ValidateDto(dto, "ParkingCategoryId");

                if (dto.ParkingCategoryId <= 0)
                    throw new ArgumentException("El campo ParkingCategoryId debe ser mayor a 0.");

                if (!string.IsNullOrWhiteSpace(dto.Location))
                {
                    dto.Location = dto.Location.Trim();
                    if (dto.Location.Length > 150)
                        throw new ArgumentException("El campo Location no puede tener más de 150 caracteres.");

                    if (!System.Text.RegularExpressions.Regex.IsMatch(dto.Location, @"^[a-zA-Z0-9áéíóúÁÉÍÓÚñÑ\s\.,-]+$"))
                        throw new ArgumentException("El campo Location contiene caracteres no permitidos.");

                    // 🔹 Unicidad por ubicación (case-insensitive)
                    var loc = dto.Location.ToLower();
                    var locationExistente = await _data.ExistsAsync(p =>
                        p.Asset && p.Location.ToLower() == loc);

                    if (locationExistente)
                        throw new InvalidOperationException($"Ya existe un parking registrado en la ubicación '{dto.Location}'.");
                }

                // 🔹 (Opcional pero recomendable) Validar FK existente para un error más claro
                //    Solo si tienes ParkingCategory en tu DbContext.
                //    Si no quieres tocar Data aquí, crea IParkingCategoryData.ExistsAsync(...)
                // bool categoriaExiste = await _context.ParkingCategories
                //     .AsNoTracking()
                //     .AnyAsync(c => c.Id == dto.ParkingCategoryId && c.Asset);
                // if (!categoriaExiste)
                //     throw new InvalidOperationException($"No existe ParkingCategory con Id {dto.ParkingCategoryId}.");

                dto.Asset = true;

                BaseModel entity = _mapper.Map<Parking>(dto);
                entity = await _data.Save((Parking)entity);

                return _mapper.Map<ParkingDto>(entity);
            }
            catch (InvalidOperationException invOe)
            {
                throw new InvalidOperationException($"Error: {invOe.Message}", invOe);
            }
            catch (ArgumentException argEx)
            {
                throw new ArgumentException($"Error: {argEx.Message}");
            }
            catch (DbUpdateException dbEx)
            {
                // 🔹 Si falla FK u otro constraint, lo hacemos explícito
                throw new BusinessException("Error de base de datos al crear el parking (¿FK ParkingCategoryId existe?).", dbEx);
            }
            catch (Exception ex)
            {
                throw new BusinessException("Error al crear el registro de parking.", ex);
            }
        }


        public override async Task Update(ParkingDto dto)
        {
            try
            {
                // Campos obligatorios para actualizar
                Validations.ValidateDto(dto, "Id", "ParkingCategoryId");

                if (dto.Id <= 0)
                    throw new ArgumentException("El campo Id debe ser mayor a 0.");

                if (dto.ParkingCategoryId <= 0)
                    throw new ArgumentException("El campo ParkingCategoryId debe ser mayor a 0.");

                // Verificar existencia del registro
                var existing = await _data.GetById(dto.Id);
                if (existing == null)
                    throw new InvalidOperationException($"No existe un parking con Id {dto.Id}.");

                if (!existing.Asset)
                    throw new InvalidOperationException("No se puede actualizar un registro de parking deshabilitado.");

                // Normalizar y validar Location si viene informada
                if (!string.IsNullOrWhiteSpace(dto.Location))
                {
                    dto.Location = dto.Location.Trim();

                    if (dto.Location.Length > 150)
                        throw new ArgumentException("El campo Location no puede tener más de 150 caracteres.");

                    if (!System.Text.RegularExpressions.Regex.IsMatch(dto.Location, @"^[a-zA-Z0-9áéíóúÁÉÍÓÚñÑ\s\.,-]+$"))
                        throw new ArgumentException("El campo Location contiene caracteres no permitidos.");

                    // Unicidad case-insensitive excluyendo el propio Id
                    var loc = dto.Location.ToLower();
                    var duplicada = await _data.ExistsAsync(p =>
                        p.Asset && p.Id != dto.Id && p.Location.ToLower() == loc
                    );
                    if (duplicada)
                        throw new InvalidOperationException($"Ya existe otro parking activo en la ubicación '{dto.Location}'.");
                }

                // (Opcional) Verificar FK de categoría antes de persistir para error claro.
                // bool categoriaExiste = await _context.ParkingCategories
                //     .AsNoTracking()
                //     .AnyAsync(c => c.Id == dto.ParkingCategoryId && c.Asset);
                // if (!categoriaExiste)
                //     throw new InvalidOperationException($"No existe ParkingCategory con Id {dto.ParkingCategoryId}.");

                // Preservar flags que no deberían mutar por accidente
                dto.Asset = existing.Asset; // no permitir que un update "apague" sin intención

                // Mapear y persistir
                var entity = _mapper.Map<Parking>(dto);
                await _data.Update(entity);
            }
            catch (InvalidOperationException invOe)
            {
                throw new InvalidOperationException($"Error: {invOe.Message}", invOe);
            }
            catch (ArgumentException argEx)
            {
                throw new ArgumentException($"Error: {argEx.Message}");
            }
            catch (DbUpdateException dbEx)
            {
                // FK / restricciones DB
                throw new BusinessException("Error de base de datos al actualizar el parking (¿FK ParkingCategoryId existe?).", dbEx);
            }
            catch (Exception ex)
            {
                throw new BusinessException("Error al actualizar el registro de parking.", ex);
            }
        }


    }
}
