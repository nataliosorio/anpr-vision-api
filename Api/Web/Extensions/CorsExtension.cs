namespace Web.Extensions
{
    public static class CorsExtension
    {
        public static IServiceCollection AddCustomCors(this IServiceCollection services, IConfiguration configuration)
        {
            var origenesPermitidos = configuration.GetValue<string>("OrigenesPermitidos")?
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy", policy =>
                {
                    policy
                        .SetIsOriginAllowed(origin =>
                        {
                            if (string.IsNullOrEmpty(origin))
                                return true; // permite origin null (Android WebView)

                            // Si coincide con uno de los configurados, permitir
                            if (origenesPermitidos?.Contains(origin) == true)
                                return true;

                            // Capacitor/Ionic origins
                            if (origin.StartsWith("capacitor://") || origin.StartsWith("ionic://"))
                                return true;

                            return false;
                        })
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                });
            });


            return services;
        }
    }
}
