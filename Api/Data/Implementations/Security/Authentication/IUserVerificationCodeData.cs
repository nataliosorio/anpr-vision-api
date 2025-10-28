using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data.Interfaces;
using Entity.Models.Security.Authentication;

namespace Data.Implementations.Security.Authentication
{
    public interface IUserVerificationCodeData : IRepositoryData<UserVerificationCode>
    {
        /// <summary>
        /// Obtiene el código de verificación activo para un usuario y tipo de código específico.
        /// </summary>
        /// <param name="userId">Id del usuario.</param>
        /// <param name="codeType">Tipo de código.</param>
        /// <returns>El código activo o null si no existe.</returns>
        Task<UserVerificationCode?> GetActiveCodeAsync(int userId, string codeType);
    }



}
