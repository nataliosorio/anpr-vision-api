using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Models.Parameter
{
    public class Camera : GenericModel
    {
        public string Resolution { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public int ParkingId { get; set; }
        public Parking? Parking { get; set; } = null!;
    }
}
