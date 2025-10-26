using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Utilities.Helpers
{
    public static class ClaimsPrincipalExtensions
    {
        public static int? GetParkingId(this ClaimsPrincipal user)
        {
            var val = user?.FindFirst("parkingId")?.Value;
            return int.TryParse(val, out var id) ? id : (int?)null;
        }

        public static int? GetUserId(this ClaimsPrincipal user)
        {
            var val = user?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(val, out var id) ? id : (int?)null;
        }
    }
}
