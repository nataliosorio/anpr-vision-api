using Entity.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Dtos.Parameter
{
    public class SlotsDto:GenericDto
    {
        public bool IsAvailable { get; set; } = true;
        public int SectorsId { get; set; }

        public string? Sectors { get; set; }
    }
}
