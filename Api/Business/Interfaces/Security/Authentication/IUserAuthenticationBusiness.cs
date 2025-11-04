using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entity.Dtos.Login;
using Entity.Dtos.Security;
using Entity.Models;

namespace Business.Interfaces.Security.Authentication
{
    /// <summary>
    /// Define los contratos para la autenticación de usuarios:
    /// - Validar credenciales y generar token JWT
    /// - Verificar acceso a un estacionamiento específico
    /// - Generar un token asociado a un estacionamiento
    /// </summary>
    public interface IUserAuthenticationBusiness
    {
        /// <summary>
        /// Paso 1️⃣ — Valida las credenciales de acceso y envía un código OTP al correo del usuario.
        /// </summary>
        /// <param name="username">Nombre de usuario.</param>
        /// <param name="password">Contraseña en texto plano.</param>
        /// <returns>
        /// <see cref="ApiResponse{T}"/> con el <c>userId</c> si las credenciales son correctas
        /// y el código fue enviado con éxito.
        /// </returns>
        Task<ApiResponse<object>> LoginWith2FAAsync(string username, string password);

        /// <summary>
        /// Paso 2️⃣ — Verifica el código OTP y genera el token JWT,
        /// devolviendo toda la información del usuario autenticado.
        /// </summary>
        /// <param name="dto">Datos de verificación (userId + code).</param>
        /// <returns>
        /// <see cref="ApiResponse{T}"/> con un <see cref="UserResponseDto"/> completo
        /// que incluye roles, roles por parking, cliente, token JWT, etc.
        /// </returns>
        Task<ApiResponse<UserResponseDto>> VerifyOtpAndGenerateTokenAsync(VerificationRequestDto dto);

        /// <summary>
        /// Verifica si un usuario tiene acceso a un parking específico.
        /// </summary>
        /// <param name="userId">Id del usuario autenticado.</param>
        /// <param name="parkingId">Id del parking a validar.</param>
        /// <returns><c>true</c> si el usuario tiene acceso; en caso contrario, <c>false</c>.</returns>
        Task<bool> ValidateUserParkingAccessAsync(int userId, int parkingId);

        /// <summary>
        /// Genera un nuevo token JWT embebiendo el identificador del parking seleccionado.
        /// </summary>
        /// <param name="userId">Id del usuario autenticado.</param>
        /// <param name="parkingId">Id del parking seleccionado.</param>
        /// <returns>Token JWT con el claim adicional <c>parkingId</c>.</returns>
        Task<string> GenerateTokenWithParkingAsync(int userId, int parkingId);
    }

}
