# ── Service binding config ─────────────────────────────────────────────────
# These match the .NET Core service URLs exactly

SERVICES = {
    "identity":  "http://localhost:5000",   # identity_service  — auth
    "inventory": "http://localhost:5001",   # inventory_service — flights/seats
    "booking":   "http://localhost:5002",   # booking_service   — bookings/payments
}

# Input routes (READ from .NET Core)
INPUT_ROUTES = {
    "all_flights":   f"{SERVICES['inventory']}/flights",
    "inventory":     f"{SERVICES['inventory']}/flights/inventory",
    "search":        f"{SERVICES['inventory']}/flights/search",   # ?departure=&arrival=
    "get_flight":    f"{SERVICES['inventory']}/flights/{{id}}",
}

# Output routes (WRITE back to .NET Core — used to push cleaned/enriched data)
OUTPUT_ROUTES = {
    "book_seat":     f"{SERVICES['booking']}/bookings",
    "cancel":        f"{SERVICES['booking']}/bookings/{{id}}",
    "get_bookings":  f"{SERVICES['booking']}/bookings/by-email/{{email}}",
}

# MongoDB — pipeline writes results here directly
MONGO = {
    "uri":      "mongodb://localhost:27017",
    "database": "indigo_inventory",
    "collections": {
        "flights":          "flights",
        "bookings":         "bookings",
        "pipeline_reports": "pipeline_reports",
        "outliers":         "outlier_reports",
    }
}

# Pipeline settings
OUTLIER_IQR_MULTIPLIER = 1.5   # standard Tukey fence
REPORT_KEEP_DAYS       = 30    # purge reports older than N days
