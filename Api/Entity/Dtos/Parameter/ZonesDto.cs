using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Dtos.Parameter
{
    public class ZonesDto:GenericDto
    {
        public int ParkingId { get; set; }
        public string? Parking { get; set; }
    }
}
