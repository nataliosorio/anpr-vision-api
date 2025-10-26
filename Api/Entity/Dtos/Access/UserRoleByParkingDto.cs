using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Dtos.Access
{
    public class UserRoleByParkingDto
    {
        public int ParkingId { get; set; }
        public string ParkingName { get; set; } = null!;
        public int RoleId { get; set; }
        public string RoleName { get; set; } = null!;
        public bool Asset { get; set; }  
    }
}
