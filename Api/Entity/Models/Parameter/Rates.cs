using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Models.Parameter
{
    public class Rates : GenericModel
    {
  
        public string Type { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public DateTime  StarHour { get; set; }
        public DateTime EndHour { get; set; }
        public int Year { get; set; }

        public int ParkingId { get; set; }
        public int RatesTypeId { get; set; }
        public int TypeVehicleId { get; set; }

        public RatesType RatesType { get; set; } = null!;
        public TypeVehicle TypeVehicle { get; set; } = null!;
        public Parking Parking { get; set; } = null!;
    }
}
