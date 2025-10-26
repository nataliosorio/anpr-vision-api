using Entity.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Models.Security
{
    public class RolFormPermission : BaseModel
    {
        public int RolId { get; set; }
        public int FormId { get; set; }
        public int PermissionId { get; set; }

        [ForeignInclude("Name", "Description")]
        public Rol Rol { get; set; }
        [ForeignInclude("Name", "Description")]
        public Form Form { get; set; }
        [ForeignInclude("Name")]
        public Permission Permission { get; set; }

    }
}
