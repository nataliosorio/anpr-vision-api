using Entity.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Entity.Models.Security
{
    public class FormModule : BaseModel
    {
        public int FormId { get; set; }
        public int ModuleId { get; set; }

        [ForeignInclude("Name")]
        public virtual Form Form { get; set; }
        [ForeignInclude("Name")]
        public virtual Module Module { get; set; }
    }
}
