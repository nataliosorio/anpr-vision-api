using Entity.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.DtoSpecific.RolFormPermission
{
    public class RolFormPermissionDtoSpecific : BaseDto
    {
        public int RolId { get; set; }
        public string RolName { get; set; } = null!;

        public int PermissionId { get; set; }
        public string PermissionName { get; set; } = null!;

        public int FormId { get; set; }
        public string FormName { get; set; } = null!;

        // 🔹 Lista de módulos asociados al Form
        public List<ModuleDtoSpecific> Modules { get; set; } = new();
    }
}
