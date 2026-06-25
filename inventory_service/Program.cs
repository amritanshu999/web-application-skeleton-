using InventoryService.Data;
using InventoryService.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(o =>
    o.AddDefaultPolicy(p => p.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// MongoDB – singleton (MongoClient is thread-safe)
builder.Services.AddSingleton<MongoDbContext>();
builder.Services.AddScoped<IFlightService, FlightService>();
builder.Services.AddScoped<IInventoryService, InventoryBookingService>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.UseCors();
app.UseAuthorization();
app.MapControllers();

// Seed 20 IndiGo flights on first run
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<MongoDbContext>();
    await FlightSeeder.SeedAsync(db);
}

app.Run("http://localhost:5001");
