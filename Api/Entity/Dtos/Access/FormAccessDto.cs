using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Dtos.Access
{
    public class FormAccessDto
    {
        public int FormId { get; set; }
        public string FormName { get; set; }
        public string FormUrl { get; set; }
        public List<string> Permissions { get; set; } = new();
    }
}
