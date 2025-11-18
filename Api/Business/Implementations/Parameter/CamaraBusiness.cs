using AutoMapper;
using Business.Interfaces.Parameter;
using Business.Interfaces.Producer;
using Data.Interfaces.Parameter;
using Entity.Dtos.Parameter;
using Entity.Models.Parameter;
using Entity.Records;
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
        private readonly IKafkaProducerService _producer;
        public CamaraBusiness(ICamaraData data, IMapper mapper, IKafkaProducerService producer)
            : base(data, mapper)
        {
            _data = data;
            _mapper = mapper;
            _producer = producer;
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

        public override async Task<CameraDto> Save(CameraDto dto)
        {
            var result = await base.Save(dto);

            await _producer.SendCameraSyncAsync(
                new CameraSyncEventRecord(
                    "CREATE",
                    new CameraSyncPayloadRecord(
                        result.Id,
                        result.ParkingId,
                        result.Name,
                        result.Url,
                        result.Resolution,
                        result.Asset ?? false,
                        result.IsDeleted
                    )
                )
            );

            return result;
        }


        public override async Task Update(CameraDto dto)
        {
            await base.Update(dto);

            await _producer.SendCameraSyncAsync(
                new CameraSyncEventRecord(
                    "UPDATE",
                    new CameraSyncPayloadRecord(
                        dto.Id,
                        dto.ParkingId,
                        dto.Name,
                        dto.Url,
                        dto.Resolution,
                        dto.Asset ?? false,
                        dto.IsDeleted
                    )
                )
            );
        }

        public override async Task<int> Delete(int id)
        {
            var deleted = await base.Delete(id);

            await _producer.SendCameraSyncAsync(
                new CameraSyncEventRecord(
                    "DELETE",
                    new CameraSyncPayloadRecord(
                        id,
                        0,
                        "",
                        "",
                        "",
                        false,     // asset false porque ya está borrada
                        true       // isDeleted true
                    )
                )
            );

            return deleted;
        }

    }
}