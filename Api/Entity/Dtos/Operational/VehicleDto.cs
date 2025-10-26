using Entity.Models;
using Org.BouncyCastle.Asn1.Crmf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Dtos.Operational
{
    public class VehicleDto: BaseDto
    {
        public string Plate { get; set; } = string.Empty;
        public string? Color { get; set; }
        public int TypeVehicleId { get; set; }
        public int ClientId { get; set; }

        // Navegación 
        public string? Client { get; set; }
        public string? TypeVehicle { get; set; } 

        //Extra
        public int? ParkingId { get; set; }
    }
}
