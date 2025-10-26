using Entity.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Dtos.Parameter
{
    public class SectorsDto:GenericDto
    {
 
        public int Capacity { get; set; }
        public int ZonesId { get; set; }
        public int TypeVehicleId { get; set; }

        public string? Zones { get; set; }
        public string? TypeVehicle { get; set; }
    }
}
