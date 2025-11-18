using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Dtos.Operational
{
    /// <summary>
    /// DTO de respuesta para la entrada manual, incluyendo los datos del registro
    /// y el ticket de parqueo generado en formato PDF (bytes).
    /// </summary>
    public class ManualEntryResponseDto : RegisteredVehiclesDto
    {
        // Propiedad que contendrá el ticket en bytes. 
        // Se envía para que el frontend pueda manejar la descarga.
        public byte[]? TicketPdfBytes { get; set; }
    }
}
