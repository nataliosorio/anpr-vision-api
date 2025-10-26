using Entity.Models.Operational;
using Entity.Models.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Models.Parameter
{
    public class Client : GenericModel
    {
        public int  PersonId { get; set; }
        // Navigation
        public Person Person { get; set; } = null!;
        public ICollection<Vehicle> Vehicles { get; set; } = new List<Vehicle>();
    }
}
