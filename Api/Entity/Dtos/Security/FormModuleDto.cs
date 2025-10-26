using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Dtos.Security
{
    public class FormModuleDto : BaseDto
    {
        public int FormId { get; set; }
        public string? FormName { get; set; } = null!;
        public int ModuleId { get; set; }
        public string? ModuleName { get; set; } = null!;

    }
}
