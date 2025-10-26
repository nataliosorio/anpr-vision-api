using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entity.Dtos.Security;

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
        /// Autentica un usuario validando sus credenciales y genera un token JWT.
        /// </summary>
        /// <param name="username">Nombre de usuario.</param>
        /// <param name="password">Contraseña del usuario.</param>
        /// <returns>
        /// Un <see cref="UserResponseDto"/> con los datos del usuario y el token, o <c>null</c> si la autenticación falla.
        /// </returns>
        Task<UserResponseDto?> AuthenticateAsync(string username, string password);

        /// <summary>
        /// Verifica si un usuario tiene acceso a un estacionamiento específico.
        /// </summary>
        /// <param name="userId">ID del usuario.</param>
        /// <param name="parkingId">ID del estacionamiento.</param>
        /// <returns><c>true</c> si el usuario tiene acceso, de lo contrario <c>false</c>.</returns>
        Task<bool> ValidateUserParkingAccessAsync(int userId, int parkingId);

        /// <summary>
        /// Genera un token JWT asociado a un usuario y a un estacionamiento determinado.
        /// </summary>
        /// <param name="userId">ID del usuario.</param>
        /// <param name="parkingId">ID del estacionamiento.</param>
        /// <returns>El token JWT generado.</returns>
        Task<string> GenerateTokenWithParkingAsync(int userId, int parkingId);
    }
}
