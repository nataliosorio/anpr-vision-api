using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Dtos.Access
{
    public class UserAccessDto
    {
        public int UserId { get; set; }
        public List<RoleAccessDto> Roles { get; set; } = new();
    }
}
