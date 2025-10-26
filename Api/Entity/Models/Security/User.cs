using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Identity.Client;

namespace Entity.Models.Security
{
    public class User : BaseModel
    {
        public string Username { get; set; } 
        public string Email { get; set; } 
        public string Password { get; set; }
        public int PersonId { get; set; }

        public Person Person { get; set; }
    }

}
