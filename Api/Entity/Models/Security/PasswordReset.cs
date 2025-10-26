using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Models.Security
{
    public class PasswordReset
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public string Code { get; set; } = string.Empty;

        //[Column(TypeName = "timestamptz")]
        public DateTime ExpiryDate { get; set; }
        public bool Used { get; set; } = false;

        //[Column(TypeName = "timestamptz")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Relación
        public User Usuario { get; set; } = null!;



    }
}
