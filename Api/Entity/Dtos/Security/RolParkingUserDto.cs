using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Dtos.Security
{
    public class RolParkingUserDto : BaseDto
    {
        public int UserId { get; set; }
        public string? UserName { get; set; } = null!;
        public int RolId { get; set; }
        public string? RolName { get; set; } = null!;
        public int ParkingId { get; set; }
        public string? ParkingName { get; set; } = null!;
    }


}
