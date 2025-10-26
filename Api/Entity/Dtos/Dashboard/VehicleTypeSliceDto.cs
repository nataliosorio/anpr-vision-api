using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Dtos.Dashboard
{
    public class VehicleTypeSliceDto
    {
        public int TypeVehicleId { get; set; }
        public string Name { get; set; } = "";
        public int Count { get; set; }
        public double Percentage { get; set; }   // 0..100
    }
}
