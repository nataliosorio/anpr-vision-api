using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Models.Security
{
    public class Rol : GenericModel
    {
        public string Description { get; set; }
        public virtual ICollection<RolFormPermission> RolFormPermission { get; set; } = new List<RolFormPermission>();


    }
}
