using Entity.Models.Operational;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Models.Parameter
{
    public class Memberships : BaseModel
    {

        public int MembershipTypeId { get; set; }
        public int VehicleId { get; set; }        

        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }            
                                                          
        // snapshots para reportes correctos
        public decimal PriceAtPurchase { get; set; }
        public int DurationDays { get; set; }
        public string? Currency { get; set; }

        public MemberShipType MembershipType { get; set; } = null!;
        public Vehicle Vehicle { get; set; } = null!;

    }
}
