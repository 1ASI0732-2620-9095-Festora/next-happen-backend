using com.festora.nexthappen;
using com.festora.nexthappen.iam.Infrastructure.Security;
using Microsoft.EntityFrameworkCore.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Registra todos los repositorios, servicios y bases de datos.
builder.Services.AddAppServices(builder.Configuration);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var jwtKey = builder.Configuration["Jwt:Key"] ?? "DEV_ONLY_INSECURE_JWT_KEY_change_me_before_production_1234567890";
builder.Services.AddAuthentication(Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(jwtKey)),
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"] ?? "nexthappen",
            ValidateAudience = true,
            ValidAudience = builder.Configuration["Jwt:Audience"] ?? "nexthappen-users",
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });
builder.Services.AddAuthorization();

// 👇 AÑADE ESTAS LÍNEAS AQUÍ 👇
// Sustituye "PasswordHasherImplementation" por tu clase real que encripta contraseñas (ej. BCryptPasswordHasher)
builder.Services.AddScoped<com.festora.nexthappen.iam.Domain.Services.IPasswordHasher, BCryptPasswordHasher>(); 

// Registra el cliente HTTP para EventCatalogClient
builder.Services.AddHttpClient<com.festora.nexthappen.ticket.Infrastructure.Http.EventCatalogClient>();
// 👆 ---------------------- 👆

// 1. Configurar CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowMobileApp", policy =>
    {
        policy.AllowAnyOrigin() // Permitir cualquier origen (útil para localhost y emuladores)
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    
    // El primero crea la BD (si no existe) y sus tablas
    services.GetRequiredService<com.festora.nexthappen.iam.Infrastructure.Persistence.IamDbContext>().Database.EnsureCreated();
    
    // Para los demas, forzamos la creacion de sus tablas porque EnsureCreated no hace nada si la BD ya existe
    try {
        services.GetRequiredService<com.festora.nexthappen.@event.Infrastructure.Persistence.EventDbContext>()
            .Database.GetService<Microsoft.EntityFrameworkCore.Storage.IRelationalDatabaseCreator>()
            .CreateTables();
    } catch { /* Ignoramos si ya existen */ }

    try {
        services.GetRequiredService<com.festora.nexthappen.engagement.Infrastructure.Persistence.EngagementDbContext>()
            .Database.GetService<Microsoft.EntityFrameworkCore.Storage.IRelationalDatabaseCreator>()
            .CreateTables();
    } catch { }

    try {
        services.GetRequiredService<com.festora.nexthappen.ticket.Infrastructure.Persistence.TicketDbContext>()
            .Database.GetService<Microsoft.EntityFrameworkCore.Storage.IRelationalDatabaseCreator>()
            .CreateTables();
    } catch { }
}
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// 2. Usar CORS antes de mapear los controladores
app.UseCors("AllowMobileApp");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
    {
        var forecast = Enumerable.Range(1, 5).Select(index =>
                new WeatherForecast
                (
                    DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                    Random.Shared.Next(-20, 55),
                    summaries[Random.Shared.Next(summaries.Length)]
                ))
            .ToArray();
        return forecast;
    })
    .WithName("GetWeatherForecast");

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}