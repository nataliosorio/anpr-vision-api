using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Dtos.Dashboard
{
    public class OccupancyItemDto
    {
        public int Id { get; set; }           // ZoneId o SectorId
        public string Name { get; set; } = "";
        public int Occupied { get; set; }
        public int Total { get; set; }
        public int Free => Math.Max(0, Total - Occupied);
        public double Percentage { get; set; } // 0..100
    }
}
