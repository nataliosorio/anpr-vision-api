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
    /// 3. Verifica código y genera token JWT con toda la información de usuario.
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

            // Enviar código OTP
            await _verificationBusiness.GenerateAndSendCodeAsync(user.Id);
            _logger.LogInformation("Código OTP enviado a {Email}", user.Email);

            return new ApiResponse<object>(
                new { userId = user.Id },
                true,
                "Credenciales válidas. Se envió un código de verificación al correo registrado.",
                null
            );
        }

        // =====================================
        // 🔹 Paso 2: Verificar código y generar token con datos completos
        // =====================================
        public async Task<ApiResponse<UserResponseDto>> VerifyOtpAndGenerateTokenAsync(VerificationRequestDto dto)
        {
            var verificationResult = await _verificationBusiness.VerifyCodeAsync(dto);
            if (!verificationResult.Success)
                return new ApiResponse<UserResponseDto>(null, false, verificationResult.Message, null);

            var user = await _userData.GetById(dto.UserId);
            if (user == null)
                return new ApiResponse<UserResponseDto>(null, false, "Usuario no encontrado", null);

            // Obtener roles por parking y roles únicos
            var rolesByParking = await _userData.GetUserRolesAsync(user.Id);
            var roleNames = rolesByParking.Select(r => r.RoleName).Distinct().ToList();

            // Mapear entidad a DTO
            var response = _mapper.Map<UserResponseDto>(user);
            response.UserId = user.Id;
            response.Username = user.Username;
            response.Roles = roleNames;
            response.RolesByParking = rolesByParking;
            response.PersonId = user.PersonId;
            response.FirstName = user.Person?.FirstName;
            response.LastName = user.Person?.LastName;

            // Asociar cliente si aplica
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

            // ✅ Generar token JWT
            response.Token = _jwtService.GenerarToken(user, roleNames);

            // Retornar igual que antes
            return new ApiResponse<UserResponseDto>(
                response,
                true,
                "Inicio de sesión exitoso",
                null
            );
        }

        // =====================================
        // 🔹 Validación de acceso a parking
        // =====================================
        public async Task<bool> ValidateUserParkingAccessAsync(int userId, int parkingId)
        {
            var rolesByParking = await _userData.GetUserRolesAsync(userId);
            return rolesByParking.Any(r => r.ParkingId == parkingId);
        }

        // =====================================
        // 🔹 Generar nuevo token con parkingId embebido
        // =====================================
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

            var extraClaims = new Dictionary<string, string> { { "parkingId", parkingId.ToString() } };
            return _jwtService.GenerarToken(user, roleNames, extraClaims);
        }
    }
}
