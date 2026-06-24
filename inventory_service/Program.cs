using InventoryService.Data;
using InventoryService.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(o =>
    o.AddDefaultPolicy(p => p.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// JSON file store – singleton so all requests share the same in-memory list
builder.Services.AddSingleton<JsonDbContext>();
builder.Services.AddScoped<IFlightService, FlightService>();
builder.Services.AddScoped<IInventoryService, InventoryBookingService>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.UseCors();
app.UseAuthorization();
app.MapControllers();

// Seed flight data on first run
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<JsonDbContext>();
    await FlightSeeder.SeedAsync(db);
}

app.Run("http://localhost:5001");
