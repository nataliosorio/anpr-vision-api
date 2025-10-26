using Entity.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Dtos.Parameter
{
    public class MembershipsDto:BaseDto
    {
        public int MembershipTypeId { get; set; }
        public int VehicleId { get; set; }

        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        // snapshots para reportes correctos
        public decimal PriceAtPurchase { get; set; }
        public int DurationDays { get; set; }
        public string? Currency { get; set; }

        public string? MembershipType { get; set; } 
        public string? Vehicle { get; set; }
    }
}
