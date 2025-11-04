using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Dtos.Login
{
    public class VerificationRequestDto
    {
        public int UserId { get; set; }
        public string Code { get; set; } = string.Empty;
    }
}
