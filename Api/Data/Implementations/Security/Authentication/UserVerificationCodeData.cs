using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Entity.Contexts.parking;
using Entity.Contexts;
using Entity.Models.Security.Authentication;
using Microsoft.Extensions.Configuration;
using Utilities.Audit.Services;
using Utilities.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Data.Implementations.Security.Authentication
{
    public class UserVerificationCodeData : RepositoryData<UserVerificationCode>, IUserVerificationCodeData
    {
        public UserVerificationCodeData(ApplicationDbContext context,
                                        IConfiguration configuration,
                                        IAuditService audit,
                                        ICurrentUserService currentUser,
                                        IMapper mapper,
                                        IParkingContext parkingContext)
            : base(context, configuration, audit, currentUser, mapper, parkingContext) { }

        public async Task<UserVerificationCode?> GetActiveCodeAsync(int userId, string codeType)
        {
            return await _context.UserVerificationCode
                .Where(c => c.UserId == userId &&
                            c.CodeType == codeType &&
                            !c.IsUsed &&
                            c.ExpiresAt > DateTime.UtcNow)
                .OrderByDescending(c => c.CreatedAt)
                .FirstOrDefaultAsync();
        }
    }
}
