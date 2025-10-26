using Entity.Models.Operational;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Models.Parameter
{
     public class Slots : GenericModel
    {
        public bool IsAvailable { get; set; }
        public int SectorsId { get; set; }

        public Sectors Sectors { get; set; } = null!;
        public ICollection<RegisteredVehicles> RegisteredVehicles { get; set; } = new List<RegisteredVehicles>();
    }
}
