using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Models.Parameter
{
     public class ParkingCategory : GenericModel
    {
       
        public string Code { get; set; } = string.Empty;
        public string? Description { get; set; }

        // Navigation
        public ICollection<Parking> Parkings { get; set; } = new List<Parking>();
    }
}
