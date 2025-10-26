using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities.Interfaces;

namespace Utilities.Implementations
{
    public class ObtainTypeVehicle : IObtainTypeVehicle
    {
        public string GetTypeVehicleByPlate(string plate)
        {
            if (string.IsNullOrWhiteSpace(plate))
                return "Desconocido";

            plate = plate.Trim().ToUpper();

            // Carro: AAA123
            if (System.Text.RegularExpressions.Regex.IsMatch(plate, @"^[A-Z]{3}\d{3}$"))
                return "Carro";

            // Moto: AAA12A
            if (System.Text.RegularExpressions.Regex.IsMatch(plate, @"^[A-Z]{3}\d{2}[A-Z]$"))
                return "Moto";

            return "Desconocido";
        }

    }
}
