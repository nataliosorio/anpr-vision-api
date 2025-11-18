using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entity.Dtos.Operational;

namespace Entity.Dtos.vehicle
{
    public class VehicleWithStatusDto : VehicleDto
    {

        public bool IsInside { get; set; }

        public DateTime? EntryDate { get; set; }
        public string? SlotName { get; set; }
        public int? SlotId { get; set; }

        public string? TimeInside { get; set; } 
    }
}
