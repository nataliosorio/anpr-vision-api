using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Models.Operational
{
    public class BlackList: BaseModel
    {
        public string Reason { get; set; } = string.Empty;
        public DateTime RestrictionDate { get; set; }
        public int VehicleId { get; set; }

        // Navigation property
        public Vehicle Vehicle { get; set; } = null!;
    }
}
