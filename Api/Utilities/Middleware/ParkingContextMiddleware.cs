using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entity.Contexts.parking;
using Microsoft.AspNetCore.Http;

namespace Utilities.Middleware
{
    public class ParkingContextMiddleware
    {
        private readonly RequestDelegate _next;

        public ParkingContextMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, IParkingContext parkingContext)
        {
            var user = context.User;
            if (user?.Identity?.IsAuthenticated == true)
            {
               
            }

            await _next(context);
        }
    }



}
