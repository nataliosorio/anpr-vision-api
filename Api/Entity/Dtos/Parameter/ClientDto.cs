using Entity.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Dtos.Parameter
{
    public class ClientDto: GenericDto
    {
        public int PersonId { get; set; }
        // Navigation
        public string? Person { get; set; }
    }
}
