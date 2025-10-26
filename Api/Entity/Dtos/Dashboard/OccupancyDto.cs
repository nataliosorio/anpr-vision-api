using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Dtos.Dashboard
{
    public class OccupancyDto
    {
        public int Occupied { get; set; }
        public int Total { get; set; }
        public double Percentage { get; set; }   // 0..100 (2 decimales)
        public int Free => Total - Occupied;
    }
}
