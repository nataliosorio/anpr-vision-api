using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Entity.Contexts.parking
{
    public class ParkingContext : IParkingContext
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ParkingContext(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public int? ParkingId
        {
            get
            {
                var user = _httpContextAccessor.HttpContext?.User;
                var claim = user?.FindFirst("parkingId");

                // Si el claim no existe o no es un número válido, devolvemos null
                if (claim == null || !int.TryParse(claim.Value, out var id))
                    return null;

                return id;
            }
        }
    }

}