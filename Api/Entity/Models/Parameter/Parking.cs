using Entity.Models.Operational;
using Entity.Models.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Models.Parameter
{
    public class Parking : GenericModel
    {
        public string Location { get; set; } = string.Empty;
        public int ParkingCategoryId { get; set; }
    
        public ParkingCategory ParkingCategory { get; set; } = null!;
        public IEnumerable<Zones> Zones { get; set; } = new List<Zones>();
        public ICollection<Rates> Rates { get; set; } = new List<Rates>();
        public IEnumerable<Camera> Camaras { get; set; } = new List<Camera>();
        public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
        public ICollection<RolParkingUser> RolParkingUsers { get; set; } = new List<RolParkingUser>();
    }
}
