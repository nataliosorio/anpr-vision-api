using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Dtos.Dashboard
{
    public class VehicleTypeDistributionDto
    {
        public int Total { get; set; }                // total de vehículos estacionados (abiertos)
        public List<string> Labels { get; set; } = new();  // nombres de tipos (para Apex)
        public List<int> Series { get; set; } = new();     // conteos por tipo (para Apex)
        public List<VehicleTypeSliceDto> Slices { get; set; } = new(); // detalle
    }
}
