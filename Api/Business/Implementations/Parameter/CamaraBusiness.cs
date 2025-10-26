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

    public class CamaraBusiness : RepositoryBusiness<Camera, CameraDto>, ICamaraBusiness
    {
        private readonly ICamaraData _data;
        private readonly IMapper _mapper;
        public CamaraBusiness(ICamaraData data, IMapper mapper)
            : base(data, mapper)
        {
            _data = data;
            _mapper = mapper;
        }
        public async Task<IEnumerable<CameraDto>> GetAllJoinAsync()
        {
            try
            {
                IEnumerable<CameraDto> entities = await _data.GetAllJoinAsync();
                if (!entities.Any()) throw new InvalidOperationException("No se encontraron camaras.");
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
                throw new Exception("Error al obtener las camaras .", ex);
            }
        }


        public async Task<IEnumerable<CameraDto>> GetByParkingAsync(int parkingId)
        {
            try
            {
                if (parkingId <= 0)
                    throw new ArgumentException("ParkingId inválido.");

                var entities = await _data.GetByParkingAsync(parkingId);

                if (!entities.Any())
                    throw new InvalidOperationException("No se encontraron cámaras para el parqueadero especificado.");

                return entities;
            }
            catch (InvalidOperationException) { throw; }
            catch (ArgumentException) { throw; }
            catch (Exception ex)
            {
                throw new BusinessException("Error al obtener cámaras por parqueadero.", ex);
            }
        }







    }
}