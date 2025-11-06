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
                        .WithOrigins(origenesPermitidos ?? Array.Empty<string>())
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                });
            });

            return services;
        }
    }
}
