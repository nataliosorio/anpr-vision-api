using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.DtoSpecific.RolFormPermission
{
    public class FormPermissionDto
    {
        public int FormId { get; set; }
        public string FormName { get; set; } = null!;
        public List<string> Permissions { get; set; } = new();   // 👈 lista de nombres de permisos
        public List<ModuleDtoSpecific> Modules { get; set; } = new();
    }
}
