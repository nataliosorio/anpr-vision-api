using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entity.Dtos.Operational;
using QuestPDF.Fluent;
using Utilities.Interfaces.Ticket;
using Utilities.Pdf;

namespace Utilities.Implementations.Ticket
{
    public class TicketService : ITicketService
    {
        public byte[] GenerateTicketPdf(RegisteredVehiclesDto dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            var document = new TicketDocument(dto);
            return document.GeneratePdf();
        }
    }
}
