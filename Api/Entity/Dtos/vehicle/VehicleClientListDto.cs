using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entity.Dtos.Operational;

namespace Entity.Dtos.vehicle
{
    public class VehicleClientListDto : VehicleDto
    {
        public bool IsInside { get; set; }           // ✅ dentro del estacionamiento
        public int? ActiveRegisteredId { get; set; }
        public int? ActiveSlotId { get; set; }
        public string? ActiveSlotName { get; set; }
        public DateTime? ActiveEntryDate { get; set; }
    }
}
