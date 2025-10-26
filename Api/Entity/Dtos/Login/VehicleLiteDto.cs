using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Dtos.Login
{
    public class VehicleLiteDto
    {
        public int Id { get; set; }
        public string Plate { get; set; } = string.Empty;
        public string? Color { get; set; }
        public int TypeVehicleId { get; set; }
    }
}
