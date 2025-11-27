using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entity.Dtos.Operational;

namespace Utilities.Interfaces.Ticket
{
    public interface ITicketService
    {
        byte[] GenerateTicketPdf(RegisteredVehiclesDto dto);
    }
}
