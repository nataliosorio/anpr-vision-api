using AutoMapper;
using Business.Interfaces.Parameter;
using Data.Interfaces.Parameter;
using Entity.Dtos.Parameter;
using Entity.Models;
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

    public class RatesBusiness : RepositoryBusiness<Rates, RatesDto>, IRatesBusiness
    {
        private readonly IRatesData _data;
        private readonly IMapper _mapper;
        private readonly IParkingBusiness _parkingBusiness;
        private readonly IRatesTypeBusiness _ratesTypeBusiness;
        private readonly ITypeVehicleBusiness _typeVehicleBusiness;
        public RatesBusiness(IRatesData data, IMapper mapper, IParkingBusiness parkingBusiness, IRatesTypeBusiness ratesTypeBusiness, ITypeVehicleBusiness typeVehicleBusiness)
            : base(data, mapper)
        {
            _data = data;
            _mapper = mapper;
            _parkingBusiness = parkingBusiness;
            _ratesTypeBusiness = ratesTypeBusiness;
            _typeVehicleBusiness = typeVehicleBusiness;
        }
        public async Task<IEnumerable<RatesDto>> GetAllJoinAsync()
        {
            try
            {
                IEnumerable<RatesDto> entities = await _data.GetAllJoinAsync();
                if (!entities.Any()) throw new InvalidOperationException("No se encontraron tarifas.");
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
                throw new Exception("Error al obtener las tarifas.", ex);
            }
        }
        public override async Task<RatesDto> Save(RatesDto dto)
        {
            try
            { 
                Validations.ValidateDto(dto, "Type", "Name","Amount","Year","ParkingId", "RatesTypeId", "TypeVehicleId");
                if (dto.Name.Length > 50)
                    throw new ArgumentException("El nombre de la tarifa no puede contener mas de 70 caracteres.");
                Validations.ValidateRangeDate(dto.StarHour, dto.EndHour);
                ParkingDto existParking = await _parkingBusiness.GetById(dto.ParkingId)
                    ?? throw new InvalidOperationException("El parqueadero no existe.");

                RatesTypeDto existRatesType = await _ratesTypeBusiness.GetById(dto.RatesTypeId)
                    ?? throw new InvalidOperationException("El tipo de tarifa no existe.");

                TypeVehicleDto existTypeVehicle = await _typeVehicleBusiness.GetById(dto.TypeVehicleId)
                    ?? throw new InvalidOperationException("El tipo de vehículo no existe.");

                dto.Asset = true;

                BaseModel entity = _mapper.Map<Rates>(dto);
                entity = await _data.Save((Rates)entity);

                return _mapper.Map<RatesDto>(entity);
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

        public override async Task Update(RatesDto dto)
        {
            try
            {
                Validations.ValidateDto(dto,"Type", "Name", "Amount", "Year", "ParkingId", "RatesTypeId", "TypeVehicleId");
                if (dto.Id <= 0)
                    throw new ArgumentException("No ha seleccioando ninguna tarifa.");
                Rates RatesExistente = await _data.GetById(dto.Id) ?? throw new InvalidOperationException($"Seleccone una tarifa válida.");
                if (dto.Name.Length > 50)
                    throw new ArgumentException("El nombre de la tarifa no puede contener mas de 70 caracteres.");

                Validations.ValidateRangeDate(dto.StarHour, dto.EndHour);

                ParkingDto existParking = await _parkingBusiness.GetById(dto.ParkingId)
                    ?? throw new InvalidOperationException("El parqueadero no existe.");

                RatesTypeDto existRatesType = await _ratesTypeBusiness.GetById(dto.RatesTypeId)
                    ?? throw new InvalidOperationException("El tipo de tarifa no existe.");

                TypeVehicleDto existTypeVehicle = await _typeVehicleBusiness.GetById(dto.TypeVehicleId)
                    ?? throw new InvalidOperationException("El tipo de vehículo no existe.");

                BaseModel entity = _mapper.Map<Rates>(dto);
                await _data.Update((Rates)entity);
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

        //public async Task<IEnumerable<RatesDto>> GetByParkingAsync(int parkingId)
        //{
        //    try
        //    {
        //        if (parkingId <= 0)
        //            throw new ArgumentException("Debe especificar un ID de parqueadero válido.");

        //        var rates = await _data.GetByParkingAsync(parkingId);
        //        if (!rates.Any())
        //            throw new InvalidOperationException("No se encontraron tarifas para este parqueadero.");

        //        return rates;
        //    }
        //    catch (InvalidOperationException invEx)
        //    {
        //        throw new InvalidOperationException($"Error: {invEx.Message}", invEx);
        //    }
        //    catch (ArgumentException argEx)
        //    {
        //        throw new ArgumentException($"Error: {argEx.Message}");
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new BusinessException("Error al obtener las tarifas por parqueadero.", ex);
        //    }
        //}

    }
}
