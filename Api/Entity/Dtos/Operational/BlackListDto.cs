using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Dtos.Operational
{
    public class BlackListDto: BaseDto
    {   
        public string Reason { get; set; } = string.Empty;
        public DateTime? RestrictionDate { get; set; }
        public int VehicleId { get; set; }
        public string? Vehicle { get; set; }
    }
}

