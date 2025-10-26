using Entity.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Dtos.Parameter
{
    public class CameraDto : GenericDto
    {
        public string Resolution { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public int ParkingId { get; set; }
        public string? Parking { get; set; } 
    }
}
