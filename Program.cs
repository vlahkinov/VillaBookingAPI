using Microsoft.EntityFrameworkCore;
using VillaBookingAPI.Data;
using VillaBookingAPI.Middleware;
using VillaBookingAPI.Services;

var builder = WebApplication.CreateBuilder(args);

// ----- Services -----

// Entity Framework Core with SQL Server
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register application services via DI
builder.Services.AddScoped<IBookingService, BookingService>();

// Controllers with JSON options
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.WriteIndented = true;
    });

// Swagger / OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "VillaBookingAPI",
        Version = "v1",
        Description = "REST API for villa booking management. Designed to sync booking data with a .NET MAUI mobile application."
    });
});

// CORS – allow the MAUI app (and dev tools) to call the API
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// ----- Middleware pipeline -----

// Global exception handler – catches unhandled errors across all controllers
app.UseGlobalExceptionHandling();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "VillaBookingAPI v1");
        c.RoutePrefix = "swagger";
    });
}

// ВАЖНО: Коментираме HTTPS пренасочването за development с физическо устройство.
// При production разкоментирайте реда по-долу и ползвайте валиден SSL сертификат.
// app.UseHttpsRedirection();

app.UseCors("AllowAll");
app.UseAuthorization();
app.MapControllers();

app.Run();