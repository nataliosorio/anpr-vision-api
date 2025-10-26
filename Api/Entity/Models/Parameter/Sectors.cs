using Entity.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Models.Parameter
{
    public class Sectors : GenericModel
    {
        public int Capacity { get; set; }
        public int ZonesId { get; set; }
        public int TypeVehicleId { get; set; }

        [ForeignInclude("Name", "Id")]
        public Zones Zones { get; set; } = null!;
        [ForeignInclude("Name", "Id")]
        public TypeVehicle TypeVehicle { get; set; } = null!;
        public ICollection<Slots> Slots { get; set; } = new List<Slots>();

    }
}
