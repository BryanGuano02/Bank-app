using Domain.Interfaces;
using Domain.Interfaces.Repositories;
using Fast_Bank.Infrastructure.Persistence;
using Fast_Bank.Infrastructure.Repositories;
using Fast_Bank.Application.Services;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // Configurar para que los enums se serialicen como strings en minúsculas
        options.JsonSerializerOptions.Converters.Add(
            new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
    });

// Configure EF Core with SQLite using the DefaultConnection string
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<DdContext>(options =>
    options.UseSqlite(connectionString));

// Expose IDdContext as a scoped dependency resolved to DdContext
builder.Services.AddScoped<IDdContext>(sp => sp.GetRequiredService<DdContext>());

// Infrastructure: repositories & unit of work
builder.Services.AddScoped<ICuentaAhorroRepository, CuentaAhorroRepository>();
builder.Services.AddScoped<ICuentaCorrienteRepository, CuentaCorrienteRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Application services
builder.Services.AddScoped<MovimientoUseCase>();
builder.Services.AddScoped<MovimientoQueryService>();
builder.Services.AddScoped<CuentaUseCase>();
builder.Services.AddScoped<CuentaQueryService>();
builder.Services.AddScoped<ClienteUseCase>();
builder.Services.AddScoped<TarjetaCreditoUseCase>();
builder.Services.AddScoped<TarjetaCreditoQueryService>();

// Domain services (used by application services)
builder.Services.AddScoped<Domain.Services.MovimientoService>();
builder.Services.AddScoped<Domain.Services.ClienteService>();
builder.Services.AddScoped<Domain.Services.CuentaService>();
builder.Services.AddScoped<Domain.Services.TarjetaCreditoService>();


// Configure OpenAPI/Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();


// Create or update the database at startup
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();

    try
    {
        var context = services.GetRequiredService<DdContext>();

        // Aplicar migraciones pendientes y crear la BD si no existe
        context.Database.Migrate();

        logger.LogInformation("Base de datos creada/actualizada exitosamente");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Error al crear la base de datos");
        throw;
    }
}


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Fast-Bank API V1");
        c.RoutePrefix = string.Empty; // serve swagger at app root (optional)
    });
}

// app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
