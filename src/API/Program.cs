using Microsoft.EntityFrameworkCore;
using Fast_Bank.Infrastructure.Persistence;
using Fast_Bank.Application.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Configure EF Core with SQLite using the DefaultConnection string
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<DdContext>(options =>
    options.UseSqlite(connectionString));

// Expose IDdContext as a scoped dependency resolved to DdContext
builder.Services.AddScoped<IDdContext>(sp => sp.GetRequiredService<DdContext>());

// Application services
builder.Services.AddScoped<MovimientoService>();

// Configure OpenAPI/Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

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
