using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.DtoSpecific.RolFormPermission
{
    public class RolFormPermissionGroupedDto
    {
        public int RolId { get; set; }
        public string RolName { get; set; } = null!;
        public List<FormPermissionDto> Forms { get; set; } = new();
    }
}
