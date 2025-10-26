using Entity.Models.Operational;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Models.Parameter
{
    public class TypeVehicle : GenericModel
    {
        // Nav
        public ICollection<Vehicle> Vehicles { get; set; } = new List<Vehicle>();
        public ICollection<Rates> Rates { get; set; } = new List<Rates>();
        public ICollection<Sectors> Sectors { get; set; } = new List<Sectors>();
    }
}
