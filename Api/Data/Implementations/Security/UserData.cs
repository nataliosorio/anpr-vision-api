using AutoMapper;
using Data.Interfaces;
using Entity.Contexts;
using Entity.Contexts.parking;
using Entity.Dtos.Access;
using Entity.Dtos.Security;
using Entity.Models.Parameter;
using Entity.Models.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities.Audit.Services;
using Utilities.Helpers;
using Utilities.Interfaces;

namespace Data.Implementations
{
    public class UserData : RepositoryData<User>, IUserData
    {
        private readonly ILogger<UserData> _logger;
        private readonly IAuditService _auditService;
        public UserData(ApplicationDbContext context, IConfiguration configuration,ILogger<UserData> logger, IAuditService auditService, ICurrentUserService currentUserService, IMapper mapper, IParkingContext parkingContext)
            : base(context, configuration, auditService, currentUserService, mapper, parkingContext)
        {
            _logger = logger;
            _auditService = auditService;
        }

        public override async Task<IEnumerable<User>> GetAll(IDictionary<string, string?>? filters = null)
        {
            try
            {
                var query = _context.Users
                    .Include(u => u.Person)
                    .AsQueryable();

                // 👉 aplicar filtros dinámicos si vienen
                if (filters != null && filters.Any())
                    query = query.ApplyFilters(filters);

                var users = await query.ToListAsync();

                //await AuditAsync("GetAll");

                return users;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los usuarios con información de la persona");
                throw;
            }
        }

        public async Task<IEnumerable<User>> GetAllByParkingAsync()
        {
            try
            {
                var parkingId = _parkingContext.ParkingId; // 👈 ID del parking actual

                var users = await (
                    from u in _context.Users.AsNoTracking()
                        .Include(u => u.Person)
                    join rpu in _context.RolParkingUsers on u.Id equals rpu.UserId
                    where rpu.ParkingId == parkingId && (u.IsDeleted == false || u.IsDeleted == null)
                    select new User
                    {
                        Id = u.Id,
                        Username = u.Username,
                        Email = u.Email,
                        PersonId = u.PersonId,
                        Person = u.Person,
                        Asset = u.Asset,
                        IsDeleted = u.IsDeleted
                    }
                ).Distinct().ToListAsync();

                return users;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener los usuarios asociados al parking actual");
                throw;
            }
        }



        public override async Task<User?> GetById(int id)
        {
            try
            {
                // Auditar acción GetById, enviamos la entidad si la encontró
                //await AuditAsync("GetById", id);
                return await _context.Users
                    .Include(u => u.Person)
                    .FirstOrDefaultAsync(u => u.Id == id);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el usuario con ID {Id}", id);
                throw;
            }
        }


        //public async Task<User?> GetUserByUsernameAsync(string username)
        //{
        //    try
        //    {
        //        //await AuditAsync("GetUserByUsernameAsync");
        //        return await _context.Set<User>()
        //            .FirstOrDefaultAsync(u => u.Username == username && u.Asset);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error al obtener usuario por nombre de usuario: {Username}", username);
        //        throw;
        //    }
        //}


        public async Task<User?> GetUserByUsernameAsync(string username)
        {
            try
            {
                return await _context.Set<User>()
                    .Include(u => u.Person)                  // <- añade esto
                    .FirstOrDefaultAsync(u => u.Username == username && u.Asset);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener usuario por nombre de usuario: {Username}", username);
                throw;
            }
        }


        public async Task<User?> GetUserByEmailsync(string email)
        {
            try
            {
                //await AuditAsync("GetUserByEmailAsync");
                return await _context.Set<User>()
                    .FirstOrDefaultAsync(u => u.Email == email && u.Asset);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener usuario por correo electrónico: {Email}", email);
                throw;
            }
        }






        public async Task<List<string>> GetUserRoleAsync(int userId)
        {
            try
            {
                // Obtener todos los roles asociados al usuario
                var userRoles = await _context.Set<RolParkingUser>()
                    .Where(ru => ru.UserId == userId)
                    .Join(
                        _context.Set<Rol>(),
                        ru => ru.RolId,
                        r => r.Id,
                        (ru, r) => r.Name
                    )
                    .ToListAsync();

                //await AuditAsync("GetUserRolesAsync", userId);

                // Si no tiene roles, devolver "Guest" como único rol
                return userRoles.Any() ? userRoles : new List<string> { "Guest" };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener roles para el usuario con ID: {UserId}", userId);
                throw;
            }
        }


        //public async Task<List<UserRoleStatusDto>> GetUserRolesAsync(int userId)
        //{
        //    try
        //    {
        //        var userRoles = await _context.Set<RolParkingUser>()
        //            .Where(ru => ru.UserId == userId)
        //            .Join(
        //                _context.Set<Rol>(),
        //                ru => ru.RolId,
        //                r => r.Id,
        //                (ru, r) => new UserRoleStatusDto
        //                {
        //                    RolUserId = ru.Id,      // ID de la tabla pivote
        //                    RoleName = r.Name,
        //                    Asset = ru.Asset
        //                }
        //            )
        //            .ToListAsync();

        //        //await AuditAsync("GetUserRolesAsync", userId);

        //        return userRoles;
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error al obtener roles para el usuario con ID: {UserId}", userId);
        //        throw;
        //    }
        //}

        public async Task<List<UserRoleByParkingDto>> GetUserRolesAsync(int userId)
        {
            try
            {
                var rolesByParking = await _context.Set<RolParkingUser>()
                    .Where(rpu => rpu.UserId == userId)
                    .Join(
                        _context.Set<Rol>(),
                        rpu => rpu.RolId,
                        r => r.Id,
                        (rpu, r) => new { rpu, r }
                    )
                    .Join(
                        _context.Set<Parking>(),
                        rr => rr.rpu.ParkingId,
                        p => p.Id,
                        (rr, p) => new UserRoleByParkingDto
                        {
                            ParkingId = p.Id,
                            ParkingName = p.Name,
                            RoleId = rr.r.Id,
                            RoleName = rr.r.Name,
                            Asset = rr.rpu.Asset
                        }
                    )
                    .ToListAsync();

                return rolesByParking;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener roles por parking para el usuario {UserId}", userId);
                throw;
            }
        }


        public async Task<UserAccessDto> GetUserAccessAsync(int userId)
        {
            try
            {
                var roles = await _context.Set<RolParkingUser>()
                .Where(ru => ru.UserId == userId)
                .Include(ru => ru.Rol)
                    .ThenInclude(r => r.RolFormPermission)
                        .ThenInclude(rfp => rfp.Form)
                            .ThenInclude(f => f.FormModules)
                                .ThenInclude(fm => fm.Module)
                .Include(ru => ru.Rol)
                    .ThenInclude(r => r.RolFormPermission)
                        .ThenInclude(rfp => rfp.Permission)
                .Select(ru => ru.Rol) // <-- el select debe ser lo último
                .ToListAsync();


                var roleDtos = roles.Select(r => new RoleAccessDto
                {
                    RoleId = r.Id,
                    RoleName = r.Name,
                    Modules = r.RolFormPermission
                        .GroupBy(rfp => rfp.Form.FormModules.First().Module)
                        .Select(m => new ModuleAccessDto
                        {
                            ModuleId = m.Key.Id,
                            ModuleName = m.Key.Name,
                            Forms = m.GroupBy(f => f.Form).Select(fg => new FormAccessDto
                            {
                                FormId = fg.Key.Id,
                                FormName = fg.Key.Name,
                                Permissions = fg.Select(p => p.Permission.Name).Distinct().ToList()
                            }).ToList()
                        }).ToList()
                }).ToList();

                return new UserAccessDto
                {
                    UserId = userId,
                    Roles = roleDtos
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener accesos para el usuario {UserId}", userId);
                throw;
            }
        }

        public async Task<User?> GetByPersonIdAsync(int personId)
        {
            var query = _context.Users.AsNoTracking();
            query = ApplyParkingFilter(query);
            return await query.FirstOrDefaultAsync(u => u.PersonId == personId);
        }









    }
}
