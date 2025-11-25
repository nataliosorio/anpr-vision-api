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
                var http = _httpContextAccessor.HttpContext;
                if (http == null)
                    return null;

                // 1️⃣ Intentar leer del claim del JWT
                var user = http.User;
                if (user?.Identity?.IsAuthenticated == true)
                {
                    var claim = user.FindFirst("parkingId");
                    if (claim != null && int.TryParse(claim.Value, out var idFromClaim))
                        return idFromClaim;
                }

                // 2️⃣ Fallback: query string ?parkingId=123
                if (http.Request.Query.TryGetValue("parkingId", out var qValues))
                {
                    var raw = qValues.FirstOrDefault();
                    if (!string.IsNullOrEmpty(raw) && int.TryParse(raw, out var idFromQuery))
                        return idFromQuery;
                }

                // 3️⃣ (Opcional) Fallback: header X-PARKING-ID
                if (http.Request.Headers.TryGetValue("X-PARKING-ID", out var hValues))
                {
                    var raw = hValues.FirstOrDefault();
                    if (!string.IsNullOrEmpty(raw) && int.TryParse(raw, out var idFromHeader))
                        return idFromHeader;
                }

                return null;
            }
        }
    }

}