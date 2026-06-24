# Indigo Identity Service - .NET Core Migration

This is a complete conversion of the Python FastAPI identity service to ASP.NET Core 8.0.

## Project Structure

```
IdentityService/
├── Models/              # Domain models and DTOs
│   ├── User.cs
│   ├── Flight.cs
│   └── SeatInventory.cs
├── Services/            # Business logic services
│   ├── AuthService.cs
│   ├── FlightService.cs
│   └── InventoryService.cs
├── Controllers/         # API endpoint controllers
│   ├── AuthController.cs
│   ├── FlightController.cs
│   └── InventoryController.cs
├── Data/               # Entity Framework DbContext
│   └── ApplicationDbContext.cs
├── Middleware/         # Custom middleware
│   └── HttpLoggingMiddleware.cs
├── Program.cs          # Application startup configuration
├── appsettings.json    # Application settings
└── IdentityService.csproj
```

## Prerequisites

- .NET 8.0 SDK installed
- Windows, macOS, or Linux

## Build & Run

### 1. Restore Dependencies
```powershell
dotnet restore
```

### 2. Build Project
```powershell
dotnet build
```

### 3. Run Application
```powershell
dotnet run
```

The application will start on `https://localhost:5001` and `http://localhost:5000`

### 4. Access Swagger UI
Open browser: `https://localhost:5001/swagger/ui/index.html`

## API Endpoints

### Authentication
- `POST /auth/register` - Register new user
  - Request: `{ "name": "string", "email": "string", "password": "string" }`
  - Response: `{ "status": "Success", "message": "string", "userId": int }`

### Flights
- `GET /flights` - Get all flights
- `GET /flights/{flightId}` - Get flight by ID
- `GET /flights/search/{departure}/{arrival}` - Search flights by route

### Inventory & Bookings
- `GET /inventory` - Get all seat inventory
- `GET /inventory/{flightId}` - Get seat inventory for flight
- `POST /inventory/book` - Book seats on flight
  - Request: `{ "flightId": "string", "passengerName": "string", "seatsRequested": int }`
- `POST /inventory/cancel/{flightId}` - Cancel booking (returns seats)
- `GET /inventory/occupancy/{flightId}` - Get flight occupancy percentage
- `POST /inventory/simulate` - Simulate time-based seat updates

## Database

SQLite database is automatically created at: `indigo_identity.db`

### Schema
- **Users** - User registration data with bcrypt-hashed passwords
- **Flights** - Flight schedules with departure/arrival times and seat capacity
- **SeatInventories** - Real-time seat availability tracking per flight

## Key Differences from Python Version

| Feature | Python | .NET Core |
|---------|--------|-----------|
| Framework | FastAPI | ASP.NET Core 8.0 |
| ORM | SQLAlchemy | Entity Framework Core 8.0 |
| Password Hashing | argon2-cffi | BCrypt.Net-Next |
| Data Validation | Pydantic v2 | Data Annotations |
| Logging | Standard logging | ILogger |
| Database | SQLite (same) | SQLite (same) |

## Environment

- Target Framework: .NET 8.0 (LTS)
- Language: C# 12
- Database: SQLite
- Package Manager: NuGet

## Logging

HTTP request logging is written to:
1. Console output
2. `server.log` file in application directory

Format: `[timestamp] LEVEL [client_ip]:[port] - "[METHOD] /path [HTTP_VERSION]" [status_code] [status_text] ([elapsed_ms]ms)`

## Development

To run in development mode with hot reload:
```powershell
dotnet watch run
```

To run tests (if added):
```powershell
dotnet test
```

## Migration Notes

- Password hashing moved from argon2 to BCrypt (more standard in .NET ecosystem)
- In-memory flight data is seeded into database on startup
- Inventory is automatically initialized from flights
- All endpoints maintain API compatibility with Python version
