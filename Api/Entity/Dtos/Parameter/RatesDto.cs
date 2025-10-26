using Entity.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Dtos.Parameter
{
    public class RatesDto:GenericDto
    {
        public string Type { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public DateTime? StarHour { get; set; }
        public DateTime? EndHour { get; set; }
        public int? Year { get; set; }

        public int ParkingId { get; set; }
        public int RatesTypeId { get; set; }
        public int TypeVehicleId { get; set; }

        public string? RatesType { get; set; }
        public string? TypeVehicle { get; set; }
        public string? Parking { get; set; }
    }
}
