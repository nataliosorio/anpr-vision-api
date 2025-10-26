using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Dtos.Access
{
    public class RoleAccessDto
    {
        public int RoleId { get; set; }
        public string RoleName { get; set; }
        public List<ModuleAccessDto> Modules { get; set; } = new();
    }
}
