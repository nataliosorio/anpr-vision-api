using Entity.Enums;
using Entity.Models.Parameter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Models.Operational
{
    public class RegisteredVehicles : BaseModel
    {
       
        public DateTime EntryDate { get; set; }
        public DateTime? ExitDate { get; set; }
        public ERegisterStatus Status { get; set; }

        public int VehicleId { get; set; }
        public Vehicle Vehicle { get; set; } = null!;

        // Opcional
        public int? SlotsId { get; set; }
        public Slots? Slots { get; set; }

    }
}
