using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Business.Interfaces.Security.Authentication;
using Data.Interfaces;
using Data.Interfaces.Parameter;
using Data.Interfaces.Security;
using Entity.Dtos.Login;
using Entity.Dtos.Security;
using Microsoft.Extensions.Logging;
using Utilities.Interfaces;

namespace Business.Implementations.Security.Authentication
{
    /// <summary>
    /// Implementa la lógica de autenticación de usuarios:
    /// - Valida credenciales
    /// - Verifica hash de contraseña (BCrypt)
    /// - Genera el token JWT
    /// - Mapea la información del usuario a un DTO de respuesta
    /// </summary>
    public class UserAuthenticationBusiness : IUserAuthenticationBusiness
    {
        private readonly IUserData _userData;
        private readonly IMapper _mapper;
        private readonly ILogger<UserAuthenticationBusiness> _logger;
        private readonly IJwtAuthenticationService _jwtService;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IClientData _clientData;

        public UserAuthenticationBusiness(
            IUserData userData,
            IMapper mapper,
            ILogger<UserAuthenticationBusiness> logger,
            IJwtAuthenticationService jwtService,
            IPasswordHasher passwordHasher,
            IClientData clientData)
        {
            _userData = userData;
            _mapper = mapper;
            _logger = logger;
            _jwtService = jwtService;
            _passwordHasher = passwordHasher;
            _clientData = clientData;
        }

        /// <summary>
        /// Autentica un usuario validando sus credenciales y generando un token JWT si son correctas.
        /// </summary>
        /// <param name="username">Nombre de usuario.</param>
        /// <param name="password">Contraseña ingresada (texto plano).</param>
        /// <returns>
        /// Un <see cref="UserResponseDto"/> con la información del usuario y el token generado,
        /// o <c>null</c> si la autenticación falla.
        /// </returns>
        public async Task<UserResponseDto?> AuthenticateAsync(string username, string password)
        {
            // Validar parámetros de entrada
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                _logger.LogWarning("Intento de autenticación con username o password vacío");
                return null;
            }
            // Buscar usuario por nombre de usuario
            var user = await _userData.GetUserByUsernameAsync(username);
            if (user == null)
            {
                _logger.LogWarning("Usuario no encontrado: {Username}", username);
                return null;
            }
            // Validar formato del hash (debe iniciar con '$2' si es BCrypt)
            if (string.IsNullOrWhiteSpace(user.Password) || !user.Password.StartsWith("$2"))
            {
                _logger.LogWarning("Formato de hash inválido para usuario {Username}", username);
                return null;
            }
            // Verificar contraseña usando el servicio de hashing
            if (!_passwordHasher.VerifyPassword(user.Password, password))
            {
                _logger.LogWarning("Contraseña incorrecta para {Username}", username);
                return null;
            }
            // Obtener roles asociados al usuario
            //var roleNames = await _userData.GetUserRoleAsync(user.Id);

            // Obtener roles detallados (por parking)
            //var rolesByParking = await _userData.GetUserRoleAsync(user.Id);
            var rolesByParking = await _userData.GetUserRolesAsync(user.Id);


            // Obtener solo los nombres de roles únicos para el JWT
            var roleNames = rolesByParking.Select(r => r.RoleName).Distinct().ToList();

            // Mapear entidad a DTO de respuesta
            var response = _mapper.Map<UserResponseDto>(user);
            //response.Roles = roleNames;
            response.Roles = roleNames;
            response.RolesByParking = rolesByParking;

            response.UserId = user.Id;
            // Generar token JWT con la información del usuario y roles
            response.Token = _jwtService.GenerarToken(user, roleNames);
            // Información adicional de la persona asociada
            response.PersonId = user.PersonId;
            response.FirstName = user.Person?.FirstName;
            response.LastName = user.Person?.LastName;

            // Asociar cliente si la persona tiene relación con uno
            if (user.PersonId > 0)
            {
                var client = await _clientData.GetClientWithVehiclesByPersonIdAsync(user.PersonId);
                if (client != null && client.PersonId == user.PersonId)
                {
                    response.Client = new ClientLiteDto
                    {
                        Id = client.Id,
                        PersonId = client.PersonId
                    };
                }
            }
            // Retornar el resultado final con el token y los datos del usuario
            return response;
        }

        public async Task<bool> ValidateUserParkingAccessAsync(int userId, int parkingId)
        {
            var rolesByParking = await _userData.GetUserRolesAsync(userId);
            return rolesByParking.Any(r => r.ParkingId == parkingId);
        }

        public async Task<string> GenerateTokenWithParkingAsync(int userId, int parkingId)
        {
            var user = await _userData.GetById(userId);
            if (user == null)
                throw new Exception("Usuario no encontrado");

            var rolesByParking = await _userData.GetUserRolesAsync(userId);
            var roleNames = rolesByParking
                .Where(r => r.ParkingId == parkingId)
                .Select(r => r.RoleName)
                .Distinct()
                .ToList();

            var extraClaims = new Dictionary<string, string>
    {
        { "parkingId", parkingId.ToString() }
    };

            return _jwtService.GenerarToken(user, roleNames, extraClaims);
        }
    }
}
