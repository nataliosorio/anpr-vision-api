using AutoMapper;
using Business.Implementations.Security.Authentication.Interfaces;
using Business.Interfaces.Security.Authentication;
using Data.Interfaces;
using Data.Interfaces.Parameter;
using Entity.Dtos.Login;
using Entity.Dtos.Security;
using Entity.Models;
using Microsoft.Extensions.Logging;
using Utilities.Interfaces;

namespace Business.Implementations.Security.Authentication
{
    /// <summary>
    /// Maneja autenticación con flujo de dos pasos (2FA por correo).
    /// 1. Valida usuario y contraseña.
    /// 2. Envía código OTP al correo.
    /// 3. Verifica código y genera token JWT.
    /// </summary>
    public class UserAuthenticationBusiness : IUserAuthenticationBusiness
    {
        private readonly IUserData _userData;
        private readonly IUserVerificationBusiness _verificationBusiness;
        private readonly IMapper _mapper;
        private readonly ILogger<UserAuthenticationBusiness> _logger;
        private readonly IJwtAuthenticationService _jwtService;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IClientData _clientData;

        public UserAuthenticationBusiness(
            IUserData userData,
            IUserVerificationBusiness verificationBusiness,
            IMapper mapper,
            ILogger<UserAuthenticationBusiness> logger,
            IJwtAuthenticationService jwtService,
            IPasswordHasher passwordHasher,
            IClientData clientData)
        {
            _userData = userData;
            _verificationBusiness = verificationBusiness;
            _mapper = mapper;
            _logger = logger;
            _jwtService = jwtService;
            _passwordHasher = passwordHasher;
            _clientData = clientData;
        }

        // =====================================
        // 🔹 Paso 1: Validar credenciales y enviar código
        // =====================================
        public async Task<ApiResponse<object>> LoginWith2FAAsync(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                return new ApiResponse<object>(null, false, "Datos incompletos", null);

            var user = await _userData.GetUserByUsernameAsync(username);
            if (user == null)
                return new ApiResponse<object>(null, false, "Usuario no encontrado", null);

            if (!_passwordHasher.VerifyPassword(user.Password, password))
                return new ApiResponse<object>(null, false, "Contraseña incorrecta", null);

            // Enviar código al correo
            await _verificationBusiness.GenerateAndSendCodeAsync(user.Id);

            _logger.LogInformation("Código OTP enviado a {Email}", user.Email);

            return new ApiResponse<object>(
                new { userId = user.Id },
                true,
                "Credenciales válidas. Código enviado al correo.",
                null
            );
        }

        // =====================================
        // 🔹 Paso 2: Verificar código y generar token
        // =====================================
        public async Task<ApiResponse<object>> VerifyOtpAndGenerateTokenAsync(VerificationRequestDto dto)
        {
            var verificationResult = await _verificationBusiness.VerifyCodeAsync(dto);
            if (!verificationResult.Success)
                return new ApiResponse<object>(null, false, verificationResult.Message, null);

            var user = await _userData.GetById(dto.UserId);
            if (user == null)
                return new ApiResponse<object>(null, false, "Usuario no encontrado", null);

            var rolesByParking = await _userData.GetUserRolesAsync(user.Id);
            var roleNames = rolesByParking.Select(r => r.RoleName).Distinct().ToList();

            var response = _mapper.Map<UserResponseDto>(user);
            response.Roles = roleNames;
            response.RolesByParking = rolesByParking;
            response.UserId = user.Id;
            response.PersonId = user.PersonId;
            response.FirstName = user.Person?.FirstName;
            response.LastName = user.Person?.LastName;

            if (user.PersonId > 0)
            {
                var client = await _clientData.GetClientWithVehiclesByPersonIdAsync(user.PersonId);
                if (client != null)
                {
                    response.Client = new ClientLiteDto
                    {
                        Id = client.Id,
                        PersonId = client.PersonId
                    };
                }
            }

            // ✅ Generar token final solo después de verificar el código
            response.Token = _jwtService.GenerarToken(user, roleNames);

            return new ApiResponse<object>(
                new { token = response.Token },
                true,
                "Verificación exitosa. Inicio de sesión completado.",
                null
            );
        }

        // =====================================
        // 🔹 Permitir token por parking
        // =====================================
        public async Task<bool> ValidateUserParkingAccessAsync(int userId, int parkingId)
        {
            var rolesByParking = await _userData.GetUserRolesAsync(userId);
            return rolesByParking.Any(r => r.ParkingId == parkingId);
        }

        public async Task<string> GenerateTokenWithParkingAsync(int userId, int parkingId)
        {
            var user = await _userData.GetById(userId);
            var rolesByParking = await _userData.GetUserRolesAsync(userId);

            var roleNames = rolesByParking
                .Where(r => r.ParkingId == parkingId)
                .Select(r => r.RoleName)
                .Distinct()
                .ToList();

            var extraClaims = new Dictionary<string, string> { { "parkingId", parkingId.ToString() } };
            return _jwtService.GenerarToken(user, roleNames, extraClaims);
        }
    }
}
