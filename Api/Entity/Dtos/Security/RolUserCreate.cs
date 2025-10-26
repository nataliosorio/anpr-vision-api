using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Dtos.Security
{
    public class RolUserCreate : BaseDto
    {
        public int RolId { get; set; }
        public int UserId { get; set; }
    }
}
