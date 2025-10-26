using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Dtos.Parameter
{
    public class ParkingDto:GenericDto
    {
        public string? Location { get; set; }
        public int ParkingCategoryId { get; set; }
        public string? ParkingCategory { get; set; } 
    }
}
