using Entity.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Dtos.Operational
{
    public class RegisteredVehiclesDto: BaseDto
    {
        public DateTime? EntryDate { get; set; }
        public DateTime? ExitDate { get; set; }

        public int VehicleId { get; set; }
        public string? Vehicle { get; set; }

        // Opcional
        public int? SlotsId { get; set; }
        public string? Slots { get; set; }
        public string? Sector {  get; set; }
    }
}
