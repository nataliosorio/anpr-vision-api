using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entity.Dtos.Access;
using Entity.Dtos.Login;

namespace Entity.Dtos.Security
{
    public class UserResponseDto
    {
        public int UserId { get; set; }
        public string Username { get; set; } = null!;
        public List<string> Roles { get; set; } = new(); // (roles globales o simplificados)
        public string Token { get; set; } = null!;

        public int PersonId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }

        public ClientLiteDto? Client { get; set; }

        // ✅ Nueva lista detallada de roles por parking
        public List<UserRoleByParkingDto> RolesByParking { get; set; } = new();

    }
}
