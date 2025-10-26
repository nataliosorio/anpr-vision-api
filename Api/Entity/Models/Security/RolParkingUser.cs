using Entity.Models.Parameter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Models.Security
{
    public class RolParkingUser : BaseModel
    {
        public int RolId { get; set; }
        public int UserId { get; set; }
        public int ParkingId { get; set; }

        public Rol? Rol { get; set; }
        public User? User { get; set; }
        public Parking? Parking { get; set; }
    }
}
