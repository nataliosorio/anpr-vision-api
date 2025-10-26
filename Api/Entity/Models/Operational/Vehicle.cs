using Entity.Dtos;
using Entity.Models.Parameter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Models.Operational
{
    public class Vehicle : BaseModel
    {
        public string Plate { get; set; } = string.Empty;
        public string? Color { get; set; }
        public int TypeVehicleId { get; set; }
        public int ClientId { get; set; }


        // Navegación (colecciones)
        public ICollection<RegisteredVehicles> RegisteredVehicles { get; set; } = new List<RegisteredVehicles>();
        public ICollection<Memberships> Memberships { get; set; } = new List<Memberships>(); // ahora requeridas
        public Client Client { get; set; } = null!;
        public TypeVehicle TypeVehicle { get; set; } = null!;
    }
}
