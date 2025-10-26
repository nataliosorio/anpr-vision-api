using AutoMapper;
using Data.Interfaces.Security;
using Entity.Contexts;
using Entity.Contexts.parking;
using Entity.Models.Security;
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
    public class ModuleData : RepositoryData<Module>, IModuleData
    {
        public ModuleData(ApplicationDbContext context, IConfiguration configuration, IAuditService auditService, ICurrentUserService currentUserService, IMapper mapper, IParkingContext parkingContext)
            : base(context, configuration, auditService, currentUserService, mapper, parkingContext)
        {

        }
    }
}
