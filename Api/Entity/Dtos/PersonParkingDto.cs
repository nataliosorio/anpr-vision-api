using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Dtos
{
    public class PersonParkingDto : BaseDto
    {
        public int PersonId { get; set; }
        public string? PersonName { get; set; } = null!;
        public int ParkingId { get; set; }
        public string? ParkingName { get; set; } = null!;

      
    }
}
