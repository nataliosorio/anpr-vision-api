using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Dtos.Security
{
    public class UserRoleStatusDto
    {
        public int RolUserId { get; set; }
        public string RoleName { get; set; } = string.Empty;
        public bool Asset { get; set; }
    }

}
