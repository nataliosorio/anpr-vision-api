using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Models.Parameter
{
    public class RatesType : GenericModel
    {
        public string? Description { get; set; }
        public ICollection<Rates> Rates { get; set; } = new List<Rates>();
    }
}
