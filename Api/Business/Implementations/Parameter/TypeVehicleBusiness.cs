using AutoMapper;
using Business.Interfaces.Parameter;
using Data.Interfaces.Parameter;
using Entity.Dtos.Dashboard;
using Entity.Dtos.Parameter;
using Entity.Models.Parameter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities.Exceptions;
using Utilities.Helpers.Validators;
using Utilities.Interfaces;

namespace Business.Implementations.Parameter
{
 
    public class TypeVehicleBusiness : RepositoryBusiness<TypeVehicle, TypeVehicleDto>, ITypeVehicleBusiness
    {
        private readonly ITypeVehicleData  _data;
        private readonly IMapper _mapper;
        private readonly IObtainTypeVehicle _obtainTypeVehicle;
        public TypeVehicleBusiness(ITypeVehicleData data, IMapper mapper,IObtainTypeVehicle obtainTypeVehicle)
            : base(data, mapper)
        {
            _data = data;
            _mapper = mapper;
            _obtainTypeVehicle = obtainTypeVehicle;
        }
        public override async Task<TypeVehicleDto> Save(TypeVehicleDto dto)
        {
            try
            {
                Validations.ValidateDto(dto, "Name");

                dto.Name = dto.Name?.Trim();

                if (string.IsNullOrWhiteSpace(dto.Name))
                    throw new ArgumentException("El campo Nombre es obligatorio.");
                if (dto.Name.Length < 3)
                    throw new ArgumentException("El nombre debe tener al menos 3 caracteres.");
                if (dto.Name.Length > 100)
                    throw new ArgumentException("El nombre no puede superar los 100 caracteres.");

                // ✅ Duplicado (case-insensitive)
                var exists = await _data.ExistsAsync(tv => tv.Name.ToLower() == dto.Name.ToLower());
                if (exists)
                    throw new ArgumentException($"Ya existe un tipo de vehículo con el nombre '{dto.Name}'.");

                dto.Asset = true;

                var entity = _mapper.Map<TypeVehicle>(dto);
                entity = await _data.Save(entity);

                return _mapper.Map<TypeVehicleDto>(entity);
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
                throw new BusinessException("Error al registrar el tipo de vehículo.", ex);
            }
        }

        public override async Task Update(TypeVehicleDto dto)
        {
            try
            {
                Validations.ValidateDto(dto, "Id", "Name");

                if (dto.Id <= 0)
                    throw new ArgumentException("El campo Id debe ser mayor que 0.");

                dto.Name = dto.Name?.Trim();

                var tipoExistente = await _data.GetById(dto.Id);
                if (tipoExistente == null)
                    throw new InvalidOperationException($"No existe un tipo de vehículo con Id {dto.Id}.");

                if (string.IsNullOrWhiteSpace(dto.Name))
                    throw new ArgumentException("El campo Nombre es obligatorio.");
                if (dto.Name.Length < 3)
                    throw new ArgumentException("El nombre debe tener al menos 3 caracteres.");
                if (dto.Name.Length > 100)
                    throw new ArgumentException("El nombre no puede superar los 100 caracteres.");

                //  Duplicado contra otros (excluyendo el propio Id)
                var existsOther = await _data.ExistsAsync(tv =>
                    tv.Name.ToLower() == dto.Name.ToLower() && tv.Id != dto.Id
                );
                if (existsOther)
                    throw new ArgumentException($"Ya existe un tipo de vehículo con el nombre '{dto.Name}'.");

                var entity = _mapper.Map<TypeVehicle>(dto);
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
            catch (Exception ex)
            {
                throw new BusinessException("Error al actualizar el tipo de vehículo.", ex);
            }
        }

        public async Task<int> GetTypeVehicleByPlate(string plate)
        {
            IEnumerable<TypeVehicle> vehicleTypes = await _data.GetAll();
            string obtainedType = _obtainTypeVehicle.GetTypeVehicleByPlate(plate);
            var match = vehicleTypes.FirstOrDefault(v => v.Name.Contains(obtainedType, StringComparison.OrdinalIgnoreCase));
            int returnType = match?.Id ?? 0;
            return returnType;
        }

    }
}
