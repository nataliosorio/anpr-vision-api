using AutoMapper;
using Business.Interfaces.Security;
using Data.Interfaces.Security;
using Entity.Dtos.Security;
using Entity.DtoSpecific.RolFormPermission;
using Entity.Models.Security;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities.Exceptions;

namespace Business.Implementations.Security
{
    public class RolFormPermissionBusiness : RepositoryBusiness<RolFormPermission, RolFormPermissionDto>, IRolFormPermissionBusiness
    {
        private readonly IRolFormPermissionData _data;
        private readonly IMapper _mapper;

        public RolFormPermissionBusiness(IRolFormPermissionData data, IMapper mapper)
            : base(data, mapper)
        {
            _data = data;
            _mapper = mapper;
        }

        public async Task<IEnumerable<RolFormPermissionDto>> GetAllJoinAsync()
        {
            var entities = await _data.GetAllJoinAsync();
            return _mapper.Map<IEnumerable<RolFormPermissionDto>>(entities);
        }

        public async Task<RolFormPermissionGroupedDto?> GetAllByRolId(int rolId)
        {
            // Traer los datos planos desde Data
            RolFormPermissionGroupedDto? groupedData = await _data.GetAllByRolId(rolId);

            if (groupedData == null)
                return null;

            return groupedData;
        }

        public override async Task<RolFormPermissionDto> Save(RolFormPermissionDto dto)
        {
            try
            {
                // Validaciones mínimas de IDs
                if (dto == null) throw new ArgumentException("Datos inválidos.");
                if (dto.PermissionId <= 0) throw new ArgumentException("Permissiond inválido.");
                if (dto.RolId <= 0) throw new ArgumentException("RolId inválido.");
                if (dto.FormId <= 0) throw new ArgumentException("FormId inválido.");

                // Evitar duplicados: mismo UserId + RolId y que no esté marcado como eliminado
                // Requiere que tu repo tenga ExistsAsync(Expression<Func<RolUser,bool>>)
                var exists = await _data.ExistsAsync(r =>
                    r.PermissionId == dto.PermissionId &&
                    r.RolId == dto.RolId && r.FormId == dto.FormId &&
                    r.IsDeleted != true    // null-safe: considera null como no eliminado
                );
                if (exists)
                    throw new ArgumentException("Este Registro ya se encuentra existente");

                // Valores por defecto para nuevo registro
                dto.Asset = true;
                dto.IsDeleted = false;

                var entity = _mapper.Map<RolFormPermission>(dto);
                var saved = await _data.Save(entity);

                return _mapper.Map<RolFormPermissionDto>(saved);
            }
            catch (ArgumentException) { throw; }
            catch (Exception ex)
            {
                throw new BusinessException("Error al registrar RolUser.", ex);
            }
        }

        public override async Task Update(RolFormPermissionDto dto)
        {
            try
            {
                if (dto == null) throw new ArgumentException("Datos inválidos.");
                if (dto.Id <= 0) throw new ArgumentException("Id inválido.");
                if (dto.PermissionId <= 0) throw new ArgumentException("PermissionId inválido.");
                if (dto.RolId <= 0) throw new ArgumentException("RolId inválido.");
                if (dto.FormId <= 0) throw new ArgumentException("FormId inválido.");

                // Verificar que exista el registro que quiero actualizar
                var current = await _data.GetById(dto.Id);
                if (current == null)
                    throw new InvalidOperationException($"No existe el registro con Id {dto.Id}.");

                // Verificar duplicado en otro registro (excluir el propio Id)
                var existsOther = await _data.ExistsAsync(r =>
                    r.Id != dto.Id &&
                    r.PermissionId == dto.PermissionId &&
                    r.RolId == dto.RolId && r.FormId == dto.FormId &&
                    r.IsDeleted != true
                );
                if (existsOther)
                    throw new ArgumentException("Este Registro ya se encuentra existente");

                // Mapear únicamente los campos que quieres actualizar (evita sobrescribir relaciones)
                current.PermissionId = dto.PermissionId;
                current.RolId = dto.RolId;

                // Si manejas Asset / IsDeleted desde DTO, actualízalos; si no, coméntalos
                current.IsDeleted = dto.IsDeleted;

                await _data.Update(current);
            }
            catch (ArgumentException) { throw; }
            catch (InvalidOperationException) { throw; }
            catch (Exception ex)
            {
                throw new BusinessException("Error al actualizar RolUser.", ex);
            }
        }

        public async Task<IEnumerable<RolFormPermissionGroupedDto>> GetAllGroupedAsync()
        {
            var data = await _data.GetAllGroupedAsync();
            return data;
        }

    }
}
