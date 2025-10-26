using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Dtos.Access
{
    public class ModuleAccessDto
    {
        public int ModuleId { get; set; }
        public string ModuleName { get; set; }
        public List<FormAccessDto> Forms { get; set; } = new();
    }
}
