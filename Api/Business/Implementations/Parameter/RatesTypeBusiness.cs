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

    public class RatesTypeBusiness : RepositoryBusiness<RatesType, RatesTypeDto>, IRatesTypeBusiness
    {
        private readonly IRatesTypeData _data;
        private readonly IMapper _mapper;
        public RatesTypeBusiness(IRatesTypeData data, IMapper mapper)
            : base(data, mapper)
        {
            _data = data;
            _mapper = mapper;
        }

        public override async Task<RatesTypeDto> Save(RatesTypeDto dto)
        {
            try
            {
                Validations.ValidateDto(dto, "Name");

                dto.Name = dto.Name?.Trim();
                if (string.IsNullOrWhiteSpace(dto.Name))
                    throw new ArgumentException("El nombre del tipo de tarifa es obligatorio.");
                if (dto.Name.Length > 50)
                    throw new ArgumentException("El nombre del tipo de tarifa no puede superar los 50 caracteres.");
                if (dto.Name.Length < 2)
                    throw new ArgumentException("El nombre del tipo de tarifa debe tener al menos 2 caracteres.");

                // Defaults tri-estado
                //if (dto.Asset == null) dto.Asset = true;
                //if (dto.IsDeleted == null) dto.IsDeleted = false;

                dto.Asset = true;
                dto.IsDeleted = false;

                // Duplicado (case-insensitive) usando ExistsAsync
                var exists = await _data.ExistsAsync(rt =>
                    rt.Name.ToLower() == dto.Name.ToLower() &&
                    (rt.IsDeleted == null || rt.IsDeleted == false)
                );
                if (exists)
                    throw new ArgumentException($"Ya existe un tipo de tarifa con el nombre '{dto.Name}'.");

                // Mapear y guardar (puedes usar AutoMapper como haces actualmente)
                var entity = _mapper.Map<RatesType>(dto);
                entity = await _data.Save(entity);

                return _mapper.Map<RatesTypeDto>(entity);
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
                throw new BusinessException("Error al crear el registro.", ex);
            }
        }



        public override async Task Update(RatesTypeDto dto)
        {
            try
            {
                Validations.ValidateDto(dto, "Id", "Name");

                if (dto.Id <= 0)
                    throw new ArgumentException("Debe seleccionar un tipo de tarifa válido.");

                dto.Name = dto.Name?.Trim();
                if (string.IsNullOrWhiteSpace(dto.Name))
                    throw new ArgumentException("El nombre del tipo de tarifa es obligatorio.");
                if (dto.Name.Length > 50)
                    throw new ArgumentException("El nombre del tipo de tarifa no puede superar los 50 caracteres.");
                if (dto.Name.Length < 2)
                    throw new ArgumentException("El nombre del tipo de tarifa debe tener al menos 2 caracteres.");

                // Obtener la entidad trackeada
                var current = await _data.GetById(dto.Id)
                             ?? throw new InvalidOperationException("El tipo de tarifa seleccionado no existe.");

              

                // Duplicado en otros registros (case-insensitive)
                var existsOther = await _data.ExistsAsync(rt =>
                    rt.Name.ToLower() == dto.Name.ToLower() &&
                    rt.Id != dto.Id &&
                    (rt.IsDeleted == null || rt.IsDeleted == false)
                );
                if (existsOther)
                    throw new ArgumentException($"Ya existe otro tipo de tarifa con el nombre '{dto.Name}'.");

                // Defaults null-safe (mantener valores actuales si dto no los trae)
                if (dto.Asset == null) dto.Asset = current.Asset;
                if (dto.IsDeleted == null) dto.IsDeleted = current.IsDeleted;

                // MAPEAR SOBRE LA INSTANCIA EXISTENTE (evita conflictos de tracking)
                // Requiere AutoMapper configurado para Map<RatesTypeDto, RatesType>
                _mapper.Map(dto, current);

                // Si no quieres AutoMapper usa asignación manual (descomenta y adapta):
                // current.Name = dto.Name;
                // current.Description = dto.Description;
                // if (dto.Asset != null) current.Asset = dto.Asset.Value;
                // if (dto.IsDeleted != null) current.IsDeleted = dto.IsDeleted.Value;

                await _data.Update(current);
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
                throw new BusinessException("Error al actualizar el registro.", ex);
            }
        }

    }
}
