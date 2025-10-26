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
  
    public class MembershipsBusiness : RepositoryBusiness<Memberships, MembershipsDto>, IMembershipsBusiness
    {
        private readonly IMembershipsData _data;
        private readonly IMapper _mapper;
        public MembershipsBusiness(IMembershipsData data, IMapper mapper)
            : base(data, mapper)
        {
            _data = data;
            _mapper = mapper;
        }

        public async Task<IEnumerable<MembershipsDto>> GetAllJoinAsync()
        {
            try
            {
                IEnumerable<MembershipsDto> entities = await _data.GetAllJoinAsync();
                if (!entities.Any()) throw new InvalidOperationException("No se encontraron membresías.");
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
                throw new Exception("Error al obtener las membresías.", ex);
            }
        }

        public override async Task<MembershipsDto> Save(MembershipsDto dto)
        {
            try
            {
                Validations.ValidateDto(dto, "MembershipTypeId", "VehicleId", "StartDate", "EndDate", "PriceAtPurchase", "DurationDays");

                if (dto.MembershipTypeId <= 0)
                    throw new ArgumentException("El campo MembershipTypeId debe ser mayor a 0.");

                if (dto.VehicleId <= 0)
                    throw new ArgumentException("El campo VehicleId debe ser mayor a 0.");

                if (dto.StartDate is null)
                    throw new ArgumentException("El campo StartDate es obligatorio.");
                if (dto.EndDate is null)
                    throw new ArgumentException("El campo EndDate es obligatorio.");

                // Normalizar como "fecha pura" (sin hora/offset)
                var start = dto.StartDate.Value.Date;
                var end = dto.EndDate.Value.Date;
                var today = DateTime.Today;

                if (start < today)
                    throw new ArgumentException("La fecha de inicio no puede ser anterior a la fecha actual.");
                if (end <= start)
                    throw new ArgumentException("La fecha de fin debe ser mayor a la fecha de inicio.");

                if (dto.PriceAtPurchase <= 0)
                    throw new ArgumentException("El precio de la membresía debe ser mayor que 0.");

                if (dto.DurationDays <= 0)
                    throw new ArgumentException("El campo DurationDays debe ser mayor que 0.");

                // fin exclusivo
                var diferenciaDias = (end - start).Days;
                if (dto.DurationDays != diferenciaDias)
                    throw new ArgumentException($"El campo DurationDays ({dto.DurationDays}) no coincide con la diferencia real de días ({diferenciaDias}).");

                if (!string.IsNullOrWhiteSpace(dto.Currency))
                {
                    dto.Currency = dto.Currency.Trim().ToUpperInvariant();
                    if (dto.Currency.Length > 3)
                        throw new ArgumentException("El campo Currency no puede tener más de 3 caracteres.");
                    if (!System.Text.RegularExpressions.Regex.IsMatch(dto.Currency, @"^[A-Z]{3}$"))
                        throw new ArgumentException("El campo Currency debe contener solo 3 letras mayúsculas (ISO 4217).");
                }

                dto.Asset = true;

                // Mapear y guardar
                BaseModel entity = _mapper.Map<Memberships>(dto);

                // Asegura que las fechas vayan sin hora
                //((Memberships)entity).StartDate = start;
                //((Memberships)entity).EndDate = end;

                // Asegura que las fechas vayan sin hora y en UTC
                ((Memberships)entity).StartDate = DateTime.SpecifyKind(start, DateTimeKind.Utc);
                ((Memberships)entity).EndDate = DateTime.SpecifyKind(end, DateTimeKind.Utc);


                entity = await _data.Save((Memberships)entity);
                return _mapper.Map<MembershipsDto>(entity);
            }
            catch (InvalidOperationException invOe) { throw new InvalidOperationException($"Error: {invOe.Message}", invOe); }
            catch (ArgumentException argEx) { throw new ArgumentException($"Error: {argEx.Message}"); }
            catch (Exception ex) { throw new BusinessException("Error al crear la membresía.", ex); }
        }


        public override async Task Update(MembershipsDto dto)
        {
            try
            {
                Validations.ValidateDto(dto, "Id", "MembershipTypeId", "VehicleId", "StartDate", "EndDate", "PriceAtPurchase", "DurationDays");

                if (dto.Id <= 0)
                    throw new ArgumentException("El campo Id debe ser mayor a 0.");

                var membershipExistente = await _data.GetById(dto.Id);
                if (membershipExistente == null)
                    throw new InvalidOperationException($"No existe una membresía con Id {dto.Id}.");

                if (!membershipExistente.Asset)
                    throw new InvalidOperationException("No se puede actualizar una membresía deshabilitada.");

                if (dto.MembershipTypeId <= 0)
                    throw new ArgumentException("El campo MembershipTypeId debe ser mayor a 0.");
                if (dto.VehicleId <= 0)
                    throw new ArgumentException("El campo VehicleId debe ser mayor a 0.");
                if (dto.StartDate is null)
                    throw new ArgumentException("El campo StartDate es obligatorio.");
                if (dto.EndDate is null)
                    throw new ArgumentException("El campo EndDate es obligatorio.");

                var start = dto.StartDate.Value.Date;
                var end = dto.EndDate.Value.Date;
                if (end <= start)
                    throw new ArgumentException("La fecha de fin debe ser mayor que la fecha de inicio.");

                if (dto.PriceAtPurchase <= 0)
                    throw new ArgumentException("El precio de la membresía debe ser mayor que 0.");
                if (dto.DurationDays <= 0)
                    throw new ArgumentException("El campo DurationDays debe ser mayor que 0.");

                var diferenciaDias = (end - start).Days; // fin exclusivo
                if (dto.DurationDays != diferenciaDias)
                    throw new ArgumentException($"El campo DurationDays ({dto.DurationDays}) no coincide con la diferencia real de días ({diferenciaDias}).");

                if (!string.IsNullOrWhiteSpace(dto.Currency))
                {
                    dto.Currency = dto.Currency.Trim().ToUpperInvariant();
                    if (dto.Currency.Length > 3)
                        throw new ArgumentException("El campo Currency no puede tener más de 3 caracteres.");
                    if (!System.Text.RegularExpressions.Regex.IsMatch(dto.Currency, @"^[A-Z]{3}$"))
                        throw new ArgumentException("El campo Currency debe contener solo 3 letras mayúsculas (ISO 4217).");
                }

                // ⚠️ Clave: mapear SOBRE la entidad ya trackeada (evita "another instance is being tracked")
                _mapper.Map(dto, membershipExistente);

                // Forzar fechas sin hora
                //membershipExistente.StartDate = start;
                //membershipExistente.EndDate = end;

                // Forzar fechas sin hora y en UTC
                membershipExistente.StartDate = DateTime.SpecifyKind(start, DateTimeKind.Utc);
                membershipExistente.EndDate = DateTime.SpecifyKind(end, DateTimeKind.Utc);


                await _data.Update(membershipExistente);
            }
            catch (InvalidOperationException invOe) { throw new InvalidOperationException($"Error: {invOe.Message}", invOe); }
            catch (ArgumentException argEx) { throw new ArgumentException($"Error: {argEx.Message}"); }
            catch (Exception ex) { throw new BusinessException("Error al actualizar la membresía.", ex); }
        }


    }
}
