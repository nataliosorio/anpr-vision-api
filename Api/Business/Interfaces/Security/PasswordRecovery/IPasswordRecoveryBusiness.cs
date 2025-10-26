using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Interfaces.Security.PasswordRecovery
{
    public interface IPasswordRecoveryBusiness
    {
        Task RequestPasswordResetAsync(string email);
        Task<bool> VerifyResetCodeAsync(string email, string code);
        Task VerifyCodeAndResetPasswordAsync(string email, string code, string newPassword);
    }
}
