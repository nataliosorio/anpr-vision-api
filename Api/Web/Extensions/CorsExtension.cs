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
                    policy.SetIsOriginAllowed(origin =>
                    {
                        if (string.IsNullOrEmpty(origin))
                            return true; // Android WebView nativa sin origin

                        // 🔹 Permitir cualquier localhost (dev, live reload, etc.)
                        if (origin.Contains("localhost"))
                            return true;

                        if (origenesPermitidos?.Any(o =>
                                string.Equals(o, origin, StringComparison.OrdinalIgnoreCase)) == true)
                            return true;

                        // 🔹 Capacitor/Ionic en APK final
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
