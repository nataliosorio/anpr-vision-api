using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilities.Pdf
{
    // Define el objeto de datos que necesita el ticket.
    // Esto asegura que la clase sea independiente de la lógica de negocio.
    public class TicketData
    {
        public string Plate { get; set; } = string.Empty;
        public string SlotName { get; set; } = string.Empty;
        public string VehicleType { get; set; } = string.Empty;   // NUEVO
        public DateTime EntryDate { get; set; }
        public int RegisteredVehicleId { get; set; }

        public string QrCodeContent => $"Entry:{RegisteredVehicleId}|Plate:{Plate}";
        public byte[] QrImageBytes { get; set; }
    }

}
