using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entity.Models.Security;

namespace Utilities.Interfaces
{
    public interface IPasswordReset
    {
        Task Add(PasswordReset reset); 

        Task<PasswordReset?> GetValidCode(int userId, string code); 

        Task MarkAsUsed(PasswordReset reset); 
        Task CleanOldResets(int days = 30);

        // ✅ Nuevos para limitar 5/h
        Task<int> CountRequestsSinceAsync(int userId, DateTime sinceUtc);
        Task<DateTime?> OldestRequestSinceAsync(int userId, DateTime sinceUtc);


    }
}
