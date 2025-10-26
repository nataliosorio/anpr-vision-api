using Confluent.Kafka;
using Entity.Contexts;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using Utilities.Implementations;
using Utilities.Interfaces;
using Utilities.Middleware;
using Web;
using Web.Config;
using Web.Extensions;
using Web.Hubs;

var builder = WebApplication.CreateBuilder(args);

/// To run a Migration in this project
/// Add-Migration InitMainDb -StartupProject Web -Project Entity -Context ApplicationDbContext
/// Update-Database -StartupProject Web -Project Entity -Context ApplicationDbContext

// 👇 Esto asegura que se lean los secrets en dev
if (builder.Environment.IsDevelopment())
{
    builder.Configuration.AddUserSecrets<Program>();
}

// Controllers
builder.Services.AddControllers();
builder.Services.AddSignalR(); // habilitar signalR

// config kafka
builder.Services.Configure<ConsumerConfig>(builder.Configuration.GetSection("Kafka"));


// Swagger
builder.Services.AddCustomSwagger();

// JWT Authentication
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddScoped<IJwtAuthenticationService, JwtAuthenticatonService>();

builder.Services.AddHttpContextAccessor();

// CORS
builder.Services.AddCustomCors(builder.Configuration);

// Extensión para la base de datos
builder.Services.AddDatabase(builder.Configuration);

// Repositorios y servicios
builder.Services.AddAppServices();

// AutoMapper
builder.Services.AddAutoMapper(typeof(AutoMapperProfiles));
//builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    // Acceso directo (si expones 5000)
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Security API v1 (directo)");

    // Vía Nginx (http://localhost:8082)
    options.SwaggerEndpoint("/api/swagger/v1/swagger.json", "Security API v1 (Nginx)");

    options.DocumentTitle = "Security API Docs";
    options.DefaultModelsExpandDepth(-1);
});

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Security API v1 (directo)");
        options.SwaggerEndpoint("/api/swagger/v1/swagger.json", "Security API v1 (Nginx)");
        options.DocumentTitle = "Security API Docs";
        options.DefaultModelsExpandDepth(-1);
    });
    app.UseDeveloperExceptionPage();
    app.MapOpenApi();
}

//app.UseHttpsRedirection();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<ParkingContextMiddleware>();

app.MapControllers();
app.MapHub<ParkingHub>("/parkingHub");

// 🔹 Redirigir "/" → "/swagger"
app.MapGet("/", context =>
{
    context.Response.Redirect("/swagger");
    return Task.CompletedTask;
});

// 🔹 Endpoint simple para healthcheck
app.MapGet("/health", () => Results.Ok(new
{
    status = "ok",
    time = DateTime.UtcNow
}));

app.Run();
