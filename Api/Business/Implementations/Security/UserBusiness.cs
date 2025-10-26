using AutoMapper;
using Business.Interfaces.Security;
using Data.Implementations;
using Data.Interfaces;
using Data.Interfaces.Parameter;
using Data.Interfaces.Security;
using Entity.Dtos.Access;
using Entity.Dtos.Login;
using Entity.Dtos.Security;
using Entity.Models;
using Entity.Models.Security;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities.Exceptions;
using Utilities.Implementations;
using Utilities.Interfaces;

namespace Business.Implementations.Security
{
    public class UserBusiness : RepositoryBusiness<User, UserDto>, IUserBusiness
    {
        private readonly IUserData _data;
        private readonly IMapper _mapper;
        private readonly ILogger<UserBusiness> _logger;
        private readonly IEmailService _emailService;
        private readonly IRolBusiness _rolBusiness;
        private readonly IRolParkingUserBusiness _rolUserBusiness;
        private readonly IJwtAuthenticationService _jwtAuthenticatonService;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IPasswordReset _passwordReset;
        private readonly IClientData _clientData;

        public UserBusiness(
        IUserData data, IMapper mapper, ILogger<UserBusiness> logger,IClientData clientData,IEmailService emailService,IRolBusiness rolBusiness, IRolParkingUserBusiness rolUserBusiness, IJwtAuthenticationService jwtAuthenticatonService, IPasswordHasher passwordHasher, IPasswordReset passwordReset) : base(data, mapper)
        {
            _data = data;
            _mapper = mapper;
            _logger = logger;
            _emailService = emailService;
            _rolBusiness = rolBusiness;
            _rolUserBusiness = rolUserBusiness;
            _jwtAuthenticatonService = jwtAuthenticatonService;
            _passwordHasher = passwordHasher;
            _passwordReset = passwordReset;
             _clientData = clientData;
        }


        public override async Task<UserDto> GetById(int id)
        {
            try
            {
                var user = await _data.GetById(id);
                if (user == null)
                    return null;

                return _mapper.Map<UserDto>(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener usuario por ID con información de persona");
                throw;
            }
        }


        public async Task<IEnumerable<UserDto>> GetAllByParkingAsync()
        {
            var users = await _data.GetAllByParkingAsync();
            return _mapper.Map<IEnumerable<UserDto>>(users);
        }

        public override async Task<UserDto> Save(UserDto dto)
        {
            try
            {
                if (await _data.ExistsAsync(x => x.Username == dto.Username))
                {
                    throw new InvalidOperationException("El nombre de usuario ya se encuentra registrado.");
                }
                // Aplicar valores predeterminados

                if (await _data.ExistsAsync(x => x.Email == dto.Email && x.Id != dto.Id))
                {
                    throw new InvalidOperationException("El correo de usuario ya se encuentra registrado.");
                }

                // 🚨 Validar que la persona no esté asociada a otro usuario
                if (dto.PersonId <= 0)
                    throw new ArgumentException("El campo PersonaId es obligatorio.");

                bool existeUsuarioParaPersona = await _data.ExistsAsync(x => x.PersonId == dto.PersonId);
                if (existeUsuarioParaPersona)
                {
                    throw new InvalidOperationException("Ya existe un usuario asociado a esta persona.");
                }



                dto.Asset = true;

                // Hashear la contraseña antes de guardar
                if (!string.IsNullOrEmpty(dto.Password))
                {
                    dto.Password = _passwordHasher.HashPassword(dto.Password);
                }

                // Mapear DTO a entidad
                User entity = _mapper.Map<User>(dto);

                // Guardar entidad en la base de datos
                entity = await _data.Save(entity);

                // Mapear la entidad guardada de vuelta a DTO
                var savedDto = _mapper.Map<UserDto>(entity);

                // Mapear la entidad guardada de vuelta a DTO y devolverla
                return savedDto;
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
                _logger.LogError(ex, "Error al crear el usuario.");
                throw new BusinessException("Error al crear el usuario.", ex);
            }
        }

       
        public async Task AssignDefaultRoleAsync(int userId)
        {
            // 1. Verificar que el usuario exista
            var user = await _data.GetById(userId);
            if (user == null)
                throw new Exception("El usuario no existe.");

            // 2. Obtener el rol por defecto ya creado
            var defaultRole = await _rolBusiness.GetByNameAsync("Usuario"); // Cambia "Usuario" por el nombre exacto si es diferente
            if (defaultRole == null)
                throw new Exception("El rol por defecto no existe.");

            // 3. Verificar si ya tiene el rol asignado
            var alreadyAssigned = await _rolUserBusiness.ExistsAsync(userId, defaultRole.Id);
            if (alreadyAssigned)
                return;

            // 4. Asignar el rol al usuario
            var userRoleDto = new RolParkingUserDto
            {
                UserId = user.Id,
                RolId = defaultRole.Id
            };

            await _rolUserBusiness.Save(userRoleDto);
        }

        public async Task SendWelcomeEmailAsync(string to)
        {
            try
            {
                string message = "¡Bienvenido! Tu usuario ha sido creado exitosamente.";

                await _emailService.SendEmailAsync(to, message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error enviando correo de bienvenida al usuario: {Email}", to);
          
            }
        }

        public async Task<List<UserRoleByParkingDto>> GetUserRolesAsync(int userId)
        {
            try
            {
                return await _data.GetUserRolesAsync(userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener roles del usuario con ID: {UserId}", userId);
                throw;
            }
        }




        public async Task<UserAccessDto> GetUserAccessAsync(int userId, bool includePermissions = true, bool includeForms = true)
        {
            try
        {
                var access = await _data.GetUserAccessAsync(userId);

                if (!includePermissions)
            {
                    foreach (var role in access.Roles)
                        foreach (var module in role.Modules)
                            foreach (var form in module.Forms)
                                form.Permissions.Clear();
            }

                if (!includeForms)
            {
                    foreach (var role in access.Roles)
                        role.Modules.Clear();
        }

                return access;
        }
            catch (Exception ex)
        {
                _logger.LogError(ex, "Error al obtener accesos del usuario con ID: {UserId}", userId);
                throw;
        }
        }



    }
}
