# IndiGo Web Application — .NET Core Microservices

A flight booking system built with 3 .NET 8 microservices.

## Architecture

| Service | Port | Description |
|---|---|---|
| `identity_service` | 5000 | User auth (login/register) + serves the web UI |
| `inventory_service` | 5001 | Flight data & seat inventory (MongoDB) |
| `booking_service` | 5002 | Bookings, payments & ticket generation |

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [MongoDB](https://www.mongodb.com/try/download/community) running on `localhost:27017`  
  *(or update the connection string in `inventory_service/appsettings.json`)*

## Run from GitHub

```bash
# 1. Clone the repo
git clone https://github.com/amritanshu999/web-application-skeleton-.git
cd web-application-skeleton-

# 2. Start all 3 services (open 3 separate terminals)

# Terminal 1 — Identity Service (port 5000)
cd identity_service
dotnet run

# Terminal 2 — Inventory Service (port 5001)
cd inventory_service
dotnet run

# Terminal 3 — Booking Service (port 5002)
cd booking_service
dotnet run

# 3. Open in browser
# http://localhost:5000
```

## MongoDB Setup

The `inventory_service` stores flight and seat data in MongoDB.

### Local MongoDB
Make sure MongoDB is running locally. Default connection string in `inventory_service/appsettings.json`:
```json
{
  "MongoDB": {
    "ConnectionString": "mongodb://localhost:27017",
    "DatabaseName": "indigo_inventory"
  }
}
```

### MongoDB Atlas (Cloud)
Replace the connection string with your Atlas URI:
```json
{
  "MongoDB": {
    "ConnectionString": "mongodb+srv://username:password@cluster.mongodb.net",
    "DatabaseName": "indigo_inventory"
  }
}
```

## Booking Flow

1. **Register / Login** at `http://localhost:5000`
2. **Search Flights** — browse 20 IndiGo routes
3. **Book** — select seats and class
4. **Pay** — scan QR code or enter UPI ID
5. **Ticket** — view/print your boarding pass with PNR
6. **My Bookings** — view all tickets, cancel bookings
