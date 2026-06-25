"""
connector.py
────────────
Binding layer between Python pipeline and .NET Core services.
All input comes through fetch_*() functions (HTTP GET from .NET).
All output goes through push_*() functions (HTTP POST/DELETE to .NET).
MongoDB is used for pipeline-only collections (reports, outliers).
"""

import requests
import pymongo
from datetime import datetime, timezone
from config import SERVICES, INPUT_ROUTES, OUTPUT_ROUTES, MONGO


# ── MongoDB client (singleton) ─────────────────────────────────────────────
_mongo_client = None

def get_db():
    global _mongo_client
    if _mongo_client is None:
        _mongo_client = pymongo.MongoClient(MONGO["uri"])
    return _mongo_client[MONGO["database"]]


# ══════════════════════════════════════════════════════════════════════════════
#  INPUT BINDINGS  — pull data from .NET Core services
# ══════════════════════════════════════════════════════════════════════════════

def fetch_all_flights() -> list[dict]:
    """GET /flights from inventory_service → list of flight dicts"""
    url = INPUT_ROUTES["all_flights"]
    try:
        r = requests.get(url, timeout=5)
        r.raise_for_status()
        return r.json()
    except Exception as e:
        print(f"  [WARN] fetch_all_flights failed ({url}): {e}")
        return []


def fetch_inventory() -> list[dict]:
    """GET /flights/inventory → seat inventory summary"""
    url = INPUT_ROUTES["inventory"]
    try:
        r = requests.get(url, timeout=5)
        r.raise_for_status()
        return r.json()
    except Exception as e:
        print(f"  [WARN] fetch_inventory failed: {e}")
        return []


def fetch_flight(flight_id: str) -> dict | None:
    """GET /flights/{id} → single flight detail"""
    url = INPUT_ROUTES["get_flight"].format(id=flight_id)
    try:
        r = requests.get(url, timeout=5)
        if r.status_code == 404:
            return None
        r.raise_for_status()
        return r.json()
    except Exception as e:
        print(f"  [WARN] fetch_flight({flight_id}) failed: {e}")
        return None


def fetch_bookings_by_email(email: str) -> list[dict]:
    """GET /bookings/by-email/{email} from booking_service"""
    url = OUTPUT_ROUTES["get_bookings"].format(email=email)
    try:
        r = requests.get(url, timeout=5)
        r.raise_for_status()
        return r.json()
    except Exception as e:
        print(f"  [WARN] fetch_bookings_by_email failed: {e}")
        return []


def fetch_bookings_from_mongo() -> list[dict]:
    """Direct MongoDB read for pipeline (bypasses HTTP for bulk analytics)"""
    db = get_db()
    return list(db[MONGO["collections"]["bookings"]].find({}, {"_id": 0}))


# ══════════════════════════════════════════════════════════════════════════════
#  OUTPUT BINDINGS  — push data back to .NET Core or MongoDB
# ══════════════════════════════════════════════════════════════════════════════

def push_booking(flight_id: str, passenger_name: str, passenger_email: str,
                 seats: int = 1, seat_class: str = "economy") -> dict | None:
    """POST /bookings to booking_service — book seats"""
    url = OUTPUT_ROUTES["book_seat"]
    payload = {
        "flightId":      flight_id,
        "passengerName": passenger_name,
        "passengerEmail":passenger_email,
        "seatsRequested": seats,
        "seatClass":     seat_class,
    }
    try:
        r = requests.post(url, json=payload, timeout=5)
        r.raise_for_status()
        return r.json()
    except Exception as e:
        print(f"  [WARN] push_booking failed: {e}")
        return None


def push_cancel(booking_id: str) -> bool:
    """DELETE /bookings/{id} — cancel a booking"""
    url = OUTPUT_ROUTES["cancel"].format(id=booking_id)
    try:
        r = requests.delete(url, timeout=5)
        return r.status_code == 200
    except Exception as e:
        print(f"  [WARN] push_cancel({booking_id}) failed: {e}")
        return False


def save_report(report_name: str, data: dict | list) -> str:
    """Write pipeline report to MongoDB pipeline_reports collection"""
    db  = get_db()
    col = db[MONGO["collections"]["pipeline_reports"]]
    doc = {
        "reportName":  report_name,
        "generatedAt": datetime.now(timezone.utc),
        "data":        data,
    }
    result = col.insert_one(doc)
    return str(result.inserted_id)


def save_outliers(outliers: list[dict], bounds: dict) -> str:
    """Write outlier records to MongoDB outlier_reports collection"""
    db  = get_db()
    col = db[MONGO["collections"]["outliers"]]
    doc = {
        "reportName":   "outlier_detection",
        "generatedAt":  datetime.now(timezone.utc),
        "iqrBounds":    bounds,
        "totalOutliers": len(outliers),
        "data":         outliers,
    }
    result = col.insert_one(doc)
    return str(result.inserted_id)


def check_services() -> dict[str, bool]:
    """Ping all .NET Core services and return their status"""
    status = {}
    for name, base in SERVICES.items():
        try:
            r = requests.get(base, timeout=3)
            status[name] = r.status_code < 500
        except:
            status[name] = False
    return status
