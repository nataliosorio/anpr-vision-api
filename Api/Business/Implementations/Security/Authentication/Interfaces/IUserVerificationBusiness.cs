using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entity.Dtos.Login;

namespace Business.Implementations.Security.Authentication.Interfaces
{
    public interface IUserVerificationBusiness
    {
        /// <summary>
        /// Genera un código de verificación para el usuario y lo envía por correo.
        /// </summary>
        /// <param name="userId">Id del usuario.</param>
        Task GenerateAndSendCodeAsync(int userId);

        /// <summary>
        /// Verifica un código de usuario.
        /// </summary>
        /// <param name="dto">DTO con la información del usuario y el código a verificar.</param>
        /// <returns>Resultado de la verificación.</returns>
        Task<VerificationResponseDto> VerifyCodeAsync(VerificationRequestDto dto);
    }
}
