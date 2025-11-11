using AutoMapper;
using Data.Interfaces.Security;
using Entity.Contexts;
using Entity.Contexts.parking;
using Entity.Models.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities.Audit.Services;
using Utilities.Interfaces;

namespace Data.Implementations.Security
{
    public class FormModuleData : RepositoryData<FormModule>, IPersonParkignData
    {
        public FormModuleData(ApplicationDbContext context, IConfiguration configuration, IAuditService auditService, ICurrentUserService currentUserService, IMapper mapper, IParkingContext parkingContext)
            : base(context, configuration,auditService, currentUserService,mapper, parkingContext)
        {

        }

        public async Task<IEnumerable<FormModule>> GetAllJoinAsync()
        {
            return await _context.FormModule
                .Include(x => x.Form)
                .Include(x => x.Module)
                 .Where(x => x.IsDeleted == false)
                   .AsNoTracking()
                .ToListAsync();
        }
    }
}
