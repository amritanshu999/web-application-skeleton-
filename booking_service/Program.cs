using BookingService.Data;
using BookingService.Services;

var builder = WebApplication.CreateBuilder(args);

// CORS – allow the identity_service frontend (port 5000) and any local origin
builder.Services.AddCors(o =>
    o.AddDefaultPolicy(p => p.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// JSON file store – singleton
builder.Services.AddSingleton<BookingDbContext>();

// HTTP client for talking to inventory_service
builder.Services.AddHttpClient<InventoryClient>(client =>
{
    var inventoryUrl = builder.Configuration["InventoryServiceUrl"] ?? "http://localhost:5001";
    client.BaseAddress = new Uri(inventoryUrl);
    client.Timeout = TimeSpan.FromSeconds(10);
});

// Business logic
builder.Services.AddScoped<IBookingService, BookingServiceImpl>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.UseCors();
app.UseAuthorization();
app.MapControllers();

app.Run("http://localhost:5002");
