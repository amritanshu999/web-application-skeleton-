using IdentityService.Data;
using IdentityService.Services;
using IdentityService.Middleware;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add CORS - allow all origins so the login page works from any host
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add Entity Framework Core
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite("Data Source=indigo_identity.db"));

// Register application services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IFlightService, FlightService>();
builder.Services.AddScoped<IInventoryService, InventoryService>();

// Add logging
builder.Services.AddLogging(config =>
{
    config.AddConsole();
    config.AddDebug();
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Only redirect to HTTPS in Development (avoids redirect failures without a cert)
if (app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}
app.UseCors();               // enable CORS
app.UseDefaultFiles();       // serves index.html at /
app.UseStaticFiles();        // serves wwwroot
app.UseAuthorization();

// Add custom HTTP logging middleware
app.UseMiddleware<HttpLoggingMiddleware>();

app.MapControllers();

// Initialize database and dummy data
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    var flightService = scope.ServiceProvider.GetRequiredService<IFlightService>();
    var inventoryService = scope.ServiceProvider.GetRequiredService<IInventoryService>();

    // Ensure database is created
    dbContext.Database.EnsureCreated();

    // Initialize dummy flights
    await flightService.InitializeDummyFlightsAsync();

    // Initialize inventory
    var flights = await dbContext.Flights.ToListAsync();
    await inventoryService.InitializeInventoryAsync(flights);
}

app.Run();
