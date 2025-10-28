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
        Task<ApiResponse<object>> LoginWith2FAAsync(string username, string password);
        Task<ApiResponse<object>> VerifyOtpAndGenerateTokenAsync(VerificationRequestDto dto);
        Task<bool> ValidateUserParkingAccessAsync(int userId, int parkingId);
        Task<string> GenerateTokenWithParkingAsync(int userId, int parkingId);
    }

}
