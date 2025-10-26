using AutoMapper;
using Business.Interfaces;
using Business.Interfaces.Operational;
using Data.Interfaces.Operational;
using Entity.Dtos.Operational;
using Entity.Models;
using Entity.Models.Operational;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities.Exceptions;
using Utilities.Helpers.Validators;

namespace Business.Implementations.Operational
{
    public class BlackListBusiness : RepositoryBusiness<BlackList, BlackListDto>, IBlackListBusiness
    {
        private readonly IBlackListData _data;
        private readonly IMapper _mapper;
        private readonly IVehicleBusiness _vehicleBusiness;
        public BlackListBusiness(IBlackListData data, IMapper mapper, IVehicleBusiness vehicleBusiness)
            : base(data, mapper)
        {
            _data = data;
            _mapper = mapper;
            _vehicleBusiness = vehicleBusiness;
        }

        //public async Task<IEnumerable<BlackListDto>> GetAllJoinAsync()
        //{
        //    try
        //    {
        //        IEnumerable<BlackListDto> entities = await _data.GetAllJoinAsync();
        //        if (!entities.Any()) throw new InvalidOperationException("No se encontraron vehiculos en la lista negra.");
        //        return entities;
        //    }
        //    catch (InvalidOperationException invEx)
        //    {
        //        throw new InvalidOperationException($"error: {invEx.Message}", invEx);
        //    }
        //    catch (ArgumentException argEx)
        //    {
        //        throw new ArgumentException($"error: {argEx.Message}", argEx);
        //    }

        //}

        //public async Task<IEnumerable<BlackListDto>> GetAllJoinAsync()
        //{
        //    try
        //    {
        //        IEnumerable<BlackListDto> entities = await _data.GetAllJoinAsync();
        //        //if (!entities.Any()) throw new InvalidOperationException("No se encontraron zonas.");
        //        //return entities;
        //        return entities ?? Enumerable.Empty<BlackListDto>();
        //    }
        //    catch (InvalidOperationException invEx)
        //    {
        //        throw new InvalidOperationException("error: ", invEx);
        //    }
        //    catch (ArgumentException argEx)
        //    {
        //        throw new ArgumentException("error: ", argEx);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception("Error al obtener las zonas .", ex);
        //    }
        //}
        public async Task<IEnumerable<BlackListDto>> GetAllJoinAsync()
        {
            var entities = await _data.GetAllJoinAsync();
            return entities ?? Enumerable.Empty<BlackListDto>();
        }

        public override async Task<BlackListDto> Save(BlackListDto dto)
        {
            try
            {
                if (dto.VehicleId <= 0)
                    throw new ArgumentException("no se selecciono ningun vehiculo.");

                if (!string.IsNullOrWhiteSpace(dto.Reason) && dto.Reason.Length > 250)
                    throw new ArgumentException("La razón no puede tener más de 250 caracteres.");

                VehicleDto existVehicle = await _vehicleBusiness.GetById(dto.VehicleId) ?? throw new InvalidOperationException("No se ha seleccionado ningún vehiculo valido");

                //  Validación usando ExistsAsync genérico
                var existe = await ExistsAsync(x => x.VehicleId == dto.VehicleId);
                if (existe)
                    throw new InvalidOperationException($"El vehículo ya está en la lista negra.");

                dto.RestrictionDate = DateTime.UtcNow;
                dto.Asset = true;

                BaseModel entity = _mapper.Map<BlackList>(dto);
                entity = await _data.Save((BlackList)entity);

                return _mapper.Map<BlackListDto>(entity);
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
                throw new BusinessException("Error al crear la entidad.", ex);
            }
        }

        public override async Task Update(BlackListDto dto)
        {
            try
            {
                if (dto.Id <= 0)
                    throw new ArgumentException("No ha seleccionado ningún registro.");
                var entityExistente = await _data.GetById(dto.Id) ?? throw new InvalidOperationException($"No se ha seleccionado ningún registro válido.");

                if (dto.VehicleId <= 0)
                {
                    var existe = await ExistsAsync(x => x.VehicleId == dto.VehicleId);
                    if (existe)
                        throw new InvalidOperationException($"El vehículo ya está en la lista negra.");
                }


                //if (existeDuplicado)
                //    throw new InvalidOperationException($"Ya existe un registro activo para el vehículo con ID {dto.VehicleId}.");

                if (!string.IsNullOrWhiteSpace(dto.Reason) && dto.Reason.Length > 250)
                    throw new ArgumentException("El campo Reason no puede tener más de 250 caracteres.");

                BaseModel entity = _mapper.Map<BlackList>(dto);
                await _data.Update((BlackList)entity);
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
                throw new BusinessException("Error al actualizar la entidad.", ex);
            }
        }



    }
}
