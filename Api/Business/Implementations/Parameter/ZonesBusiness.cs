using AutoMapper;
using Business.Interfaces.Parameter;
using Data.Interfaces.Parameter;
using Entity.Dtos.Parameter;
using Entity.Models.Parameter;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities.Exceptions;

namespace Business.Implementations.Parameter
{
 
    public class ZonesBusiness : RepositoryBusiness<Zones,ZonesDto>, IZonesBusiness
    {
        private readonly IZonesData _data;
        private readonly IMapper _mapper;
        public ZonesBusiness(IZonesData data, IMapper mapper)
            : base(data, mapper)
        {
            _data = data;
            _mapper = mapper;
        }


        public override async Task<ZonesDto> Save(ZonesDto dto)
        {
            try
            {
                // Normalización
                dto.Name = dto.Name?.Trim();

                // Reglas básicas
                if (string.IsNullOrWhiteSpace(dto.Name))
                    throw new ArgumentException("El campo 'Name' es obligatorio.");
                if (dto.ParkingId <= 0)
                    throw new ArgumentException("El 'ParkingId' es obligatorio y debe ser mayor que 0.");
                if (dto.Name.Length < 2)
                    throw new ArgumentException("El nombre debe tener al menos 2 caracteres.");
                if (dto.Name.Length > 100)
                    throw new ArgumentException("El nombre no puede superar los 100 caracteres.");

                // Duplicado: misma zona (Name) en el mismo parqueadero (excluir IsDeleted)
                var exists = await _data.ExistsAsync(z =>
                    z.ParkingId == dto.ParkingId &&
                    z.Name != null &&
                    z.Name.ToLower() == dto.Name.ToLower()
                );
                if (exists)
                    throw new ArgumentException($"Ya existe una zona con el nombre '{dto.Name}' en este parqueadero.");

                // Preparar entidad manualmente (evitamos AutoMapper en este método)
                var entity = new Zones
                {
                    Name = dto.Name!,
                    ParkingId = dto.ParkingId,
                    Asset = true,
                    IsDeleted = false

                };

                // Guardar en la BD
                entity = await _data.Save(entity);

                // Devolver DTO con datos guardados
                return new ZonesDto
                {
                    Id = entity.Id,
                    Name = entity.Name,
                    ParkingId = entity.ParkingId,
                    Parking = null,
                    Asset = entity.Asset,
                    IsDeleted = entity.IsDeleted
                };
            }
            catch (ArgumentException) { throw; }
            catch (DbUpdateException dbEx)
            {
                // Muestra traza completa temporalmente para diagnosticar
                Console.WriteLine("----- DbUpdateException en ZonesBusiness.Save -----");
                Console.WriteLine(dbEx.ToString());
                Console.WriteLine("----- end -----");

                throw new BusinessException("Error de BD al registrar la zona.", dbEx);
            }
            catch (Exception ex)
            {
                // Muestra traza completa temporalmente para diagnosticar
                Console.WriteLine("----- Exception en ZonesBusiness.Save -----");
                Console.WriteLine(ex.ToString());
                Console.WriteLine("----- end -----");

                throw new BusinessException("Error al registrar la zona.", ex);
            }
        }

        public override async Task Update(ZonesDto dto)
        {
            try
            {
                if (dto.Id <= 0)
                    throw new ArgumentException("El Id debe ser mayor que 0.");

                dto.Name = dto.Name?.Trim();

                if (string.IsNullOrWhiteSpace(dto.Name))
                    throw new ArgumentException("El campo 'Name' es obligatorio.");
                if (dto.ParkingId <= 0)
                    throw new ArgumentException("El 'ParkingId' es obligatorio y debe ser mayor que 0.");
                if (dto.Name.Length < 2)
                    throw new ArgumentException("El nombre debe tener al menos 2 caracteres.");
                if (dto.Name.Length > 100)
                    throw new ArgumentException("El nombre no puede superar los 100 caracteres.");

                // Recuperar la entidad actual
                var current = await _data.GetById(dto.Id);
                if (current == null)
                    throw new InvalidOperationException($"No existe una zona con Id {dto.Id}.");

                //if (!current.Asset)
                //    throw new InvalidOperationException("No se puede actualizar una zona deshabilitada.");

                // Duplicado en otros registros del mismo parking (excluir propio Id y eliminados)
                var existsOther = await _data.ExistsAsync(z =>
                    z.ParkingId == dto.ParkingId &&
                    z.Id != dto.Id &&
                    z.Name != null &&
                    z.Name.ToLower() == dto.Name.ToLower()
                );
                if (existsOther)
                    throw new ArgumentException($"Ya existe otra zona con el nombre '{dto.Name}' en este parqueadero.");

                // Actualizar la entidad existente (mantenemos IsDeleted del registro actual)
                current.Name = dto.Name!;
                current.ParkingId = dto.ParkingId;
                current.Asset = dto.Asset ?? current.Asset;

                // current.IsDeleted = current.IsDeleted; // mantener igual

                await _data.Update(current);
            }
            catch (ArgumentException) { throw; }
            catch (InvalidOperationException) { throw; }
            catch (DbUpdateException dbEx)
            {
                Console.WriteLine("----- DbUpdateException en ZonesBusiness.Update -----");
                Console.WriteLine(dbEx.ToString());
                Console.WriteLine("----- end -----");

                throw new BusinessException("Error de BD al actualizar la zona.", dbEx);
            }
            catch (Exception ex)
            {
                Console.WriteLine("----- Exception en ZonesBusiness.Update -----");
                Console.WriteLine(ex.ToString());
                Console.WriteLine("----- end -----");

                throw new BusinessException("Error al actualizar la zona.", ex);
            }
        }

        public async Task<IEnumerable<ZonesDto>> GetAllJoinAsync()
        {
            try
            {
                IEnumerable<ZonesDto> entities = await _data.GetAllJoinAsync();
                if (!entities.Any()) throw new InvalidOperationException("No se encontraron zonas.");
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
                throw new Exception("Error al obtener las zonas .", ex);
        }
        }

        //public async Task<IEnumerable<ZonesDto>> GetAllByParkingId(int parkingId)
        //{
        //    try
        //    {
        //        if (parkingId < 1) throw new ArgumentException("El id del estacionamiento es inv·lido.");
        //        IEnumerable<Zones> entities = await _data.GetAllByParkingId(parkingId);
        //        if (!entities.Any()) throw new InvalidOperationException("No se encontraron zonas para el estacionamiento.");
        //        return _mapper.Map<IEnumerable<ZonesDto>>(entities);
        //    }
        //    catch (InvalidOperationException invEx)
        //    {
        //        throw new InvalidOperationException("error: ", invEx);
        //    }
        //    catch (ArgumentException argEx)
        //    {
        //        throw new ArgumentException("error: ",argEx);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception("Error al obtener las zonas del estacionamiento.", ex);
        //    }
        //}
    }
}
