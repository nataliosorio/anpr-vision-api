using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Data.Interfaces.Menu;
using Entity.Contexts.parking;
using Entity.Contexts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Utilities.Audit.Services;
using Utilities.Interfaces;
using Entity.Dtos.Access;
using Microsoft.EntityFrameworkCore;

namespace Data.Implementations.Menu
{
    public class MenuData : IMenuData
    {
        private readonly ApplicationDbContext _db;

        public MenuData(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<UserAccessDto> GetUserMenuAsync(int userId, int parkingId)
        {
            // 1️⃣ Roles del usuario en el parqueadero
            var userRoles = await _db.RolParkingUsers
                .Include(rpu => rpu.Rol)
                .Where(rpu => rpu.UserId == userId && rpu.ParkingId == parkingId)
                .ToListAsync();

            var dto = new UserAccessDto
            {
                UserId = userId
            };

            // 2️⃣ Iterar roles
            foreach (var role in userRoles)
            {
                var roleDto = new RoleAccessDto
                {
                    RoleId = role.RolId,
                    RoleName = role.Rol.Name
                };

                // Formularios + Permisos asociados a ese rol
                var formPermissions = await _db.RolFormPermission
                    .Include(rfp => rfp.Form)
                        .ThenInclude(f => f.FormModules)
                            .ThenInclude(fm => fm.Module)
                    .Include(rfp => rfp.Permission)
                    .Where(rfp => rfp.RolId == role.RolId)
                    .ToListAsync();

                // Agrupar por módulo
                var groupedByModule = formPermissions
                    .GroupBy(fp => fp.Form.FormModules.FirstOrDefault()?.Module)
                    .Where(g => g.Key != null);

                foreach (var moduleGroup in groupedByModule)
                {
                    var module = moduleGroup.Key!;
                    var moduleDto = new ModuleAccessDto
                    {
                        ModuleId = module.Id,
                        ModuleName = module.Name
                    };

                    // Agrupar formularios dentro del módulo
                    var groupedForms = moduleGroup.GroupBy(fp => fp.Form);

                    foreach (var formGroup in groupedForms)
                    {
                        var form = formGroup.Key;
                        var formDto = new FormAccessDto
                        {
                            FormId = form.Id,
                            FormName = form.Name,
                            FormUrl = form.Url,
                            Permissions = formGroup
                                .Select(fp => fp.Permission.Name)
                                .Distinct()
                                .ToList()
                        };
                        
                        moduleDto.Forms.Add(formDto);
                    }

                    roleDto.Modules.Add(moduleDto);
                }

                dto.Roles.Add(roleDto);
            }

            return dto;
        }
    }
}

