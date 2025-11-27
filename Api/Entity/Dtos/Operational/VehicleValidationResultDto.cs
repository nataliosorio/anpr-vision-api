using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Dtos.Operational
{
    public class VehicleValidationResultDto
    {
        public bool Exists { get; set; }
        public bool IsBlacklisted { get; set; }
        public bool HasActiveEntry { get; set; }
        public int? TypeVehicleId { get; set; }
        public string? ClientName { get; set; }
        public string? VehicleColor { get; set; }
        public string Message { get; set; } = string.Empty; // Mensaje descriptivo para el frontend
    }
}