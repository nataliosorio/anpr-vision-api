using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Models.Security
{
    public class Form : GenericModel
    {

        public string? Description { get; set; }
        //public string? Url { get; set; }
        public virtual ICollection<FormModule> FormModules { get; set; } = new List<FormModule>();
        public virtual ICollection<RolFormPermission> RolFormPermission { get; set; } = new List<RolFormPermission>();

    }
}
