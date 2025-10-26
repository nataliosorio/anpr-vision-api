using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Models.Parameter
{
    public class MemberShipType : GenericModel
    {
        
        public string? Description { get; set; }
        public decimal PriceBase { get; set; }
        public int DurationDaysBase { get; set; }

        public ICollection<Memberships> Memberships { get; set; } = new List<Memberships>();
    }
}
