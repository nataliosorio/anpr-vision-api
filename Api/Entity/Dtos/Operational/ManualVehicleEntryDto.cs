using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Dtos.Operational
{
    public class ManualVehicleEntryDto
    {
        [Required(ErrorMessage = "La Placa es requerida.")]
        public string Plate { get; set; } = string.Empty;

        [Required(ErrorMessage = "El ID del parqueadero es requerido.")]
        [Range(1, int.MaxValue, ErrorMessage = "El ParkingId debe ser un valor válido.")]
        public int ParkingId { get; set; }

        [Required(ErrorMessage = "El ID del tipo de vehículo es requerido.")]
        [Range(1, int.MaxValue, ErrorMessage = "El TypeVehicleId debe ser mayor a 0.")]
        public int TypeVehicleId { get; set; }

        [Required(ErrorMessage = "El nombre del cliente es requerido.")]
        public string ClientName { get; set; } = string.Empty;

        [Required(ErrorMessage = "El correo del cliente es requerido.")]
        [EmailAddress(ErrorMessage = "El correo debe tener un formato válido.")]
        public string ClientEmail { get; set; } = string.Empty;
    }
}
