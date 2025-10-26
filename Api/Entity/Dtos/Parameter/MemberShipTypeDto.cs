using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Dtos.Parameter
{
    public class MemberShipTypeDto:GenericDto
    {
        public string? Description { get; set; }
        public decimal PriceBase { get; set; }
        public int DurationDaysBase { get; set; }
    }
}
