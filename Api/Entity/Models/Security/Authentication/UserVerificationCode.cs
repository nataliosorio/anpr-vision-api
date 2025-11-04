using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Models.Security.Authentication
{
    public class UserVerificationCode : BaseModel
    {
        public int UserId { get; set; }
        public string CodeHash { get; set; } = null!;
        public string CodeType { get; set; } = "LOGIN";
        public DateTime ExpiresAt { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ConsumedAt { get; set; }
        public int Attempts { get; set; } = 0;
        public bool IsUsed { get; set; } = false;

        public User User { get; set; } = null!;
    }
}
