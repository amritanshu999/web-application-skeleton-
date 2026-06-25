using MongoDB.Driver;
using MongoDB.Bson;

var conn   = "mongodb://localhost:27017";
var dbName = "indigo_inventory";

Console.WriteLine($"Connecting to {conn} ...");
var client = new MongoClient(conn);
var db     = client.GetDatabase(dbName);
var col    = db.GetCollection<BsonDocument>("flights");

// Drop existing and re-seed fresh
await col.DeleteManyAsync(FilterDefinition<BsonDocument>.Empty);
Console.WriteLine("Dropped existing flights. Seeding fresh data...");

var rnd = new Random(42);

// ── Airport master data ────────────────────────────────────────────────────
static (string name, string city, string term) AP(string code) => code switch
{
    "DEL" => ("Indira Gandhi Intl","New Delhi","T3"),
    "BOM" => ("Chhatrapati Shivaji Maharaj Intl","Mumbai","T2"),
    "BLR" => ("Kempegowda Intl","Bengaluru","T2"),
    "HYD" => ("Rajiv Gandhi Intl","Hyderabad","T1"),
    "MAA" => ("Chennai Intl","Chennai","T1"),
    "CCU" => ("Netaji Subhas Chandra Bose Intl","Kolkata","T2"),
    "GOI" => ("Dabolim Airport","Goa","T1"),
    "AMD" => ("Sardar Vallabhbhai Patel Intl","Ahmedabad","T1"),
    "PNQ" => ("Pune Airport","Pune","T1"),
    "COK" => ("Cochin Intl","Kochi","T1"),
    "JAI" => ("Jaipur Intl","Jaipur","T1"),
    "LKO" => ("Chaudhary Charan Singh Intl","Lucknow","T1"),
    "IXC" => ("Chandigarh Intl","Chandigarh","T1"),
    "ATQ" => ("Sri Guru Ram Dass Jee Intl","Amritsar","T1"),
    "SXR" => ("Sheikh ul Alam Intl","Srinagar","T1"),
    "IXL" => ("Kushok Bakula Rimpochee Airport","Leh","T1"),
    "GAU" => ("Lokpriya Gopinath Bordoloi Intl","Guwahati","T1"),
    "IXB" => ("Bagdogra Intl","Siliguri","T1"),
    "IMF" => ("Imphal Intl","Imphal","T1"),
    "DIB" => ("Dibrugarh Airport","Dibrugarh","T1"),
    "AGX" => ("Agartala Airport","Agartala","T1"),
    "BBI" => ("Biju Patnaik Intl","Bhubaneswar","T1"),
    "VNS" => ("Lal Bahadur Shastri Intl","Varanasi","T1"),
    "PAT" => ("Jay Prakash Narayan Intl","Patna","T1"),
    "NAG" => ("Dr. Babasaheb Ambedkar Intl","Nagpur","T1"),
    "IDR" => ("Devi Ahilya Bai Holkar Airport","Indore","T1"),
    "BHO" => ("Raja Bhoj Airport","Bhopal","T1"),
    "RPR" => ("Swami Vivekananda Airport","Raipur","T1"),
    "UDR" => ("Maharana Pratap Airport","Udaipur","T1"),
    "IXR" => ("Birsa Munda Airport","Ranchi","T1"),
    "TRV" => ("Trivandrum Intl","Thiruvananthapuram","T1"),
    "TRZ" => ("Tiruchirappalli Intl","Tiruchirappalli","T1"),
    "VGA" => ("Vijayawada Airport","Vijayawada","T1"),
    "TIR" => ("Tirupati Airport","Tirupati","T1"),
    "MYQ" => ("Mysore Airport","Mysuru","T1"),
    "MYS" => ("Mysore Airport","Mysuru","T1"),
    "GOX" => ("Mopa Intl Airport","North Goa","T1"),
    "IXE" => ("Mangalore Intl","Mangaluru","T1"),
    "HBX" => ("Hubli Airport","Hubballi","T1"),
    "BDQ" => ("Vadodara Airport","Vadodara","T1"),
    "STV" => ("Surat Airport","Surat","T1"),
    "JDH" => ("Jodhpur Airport","Jodhpur","T1"),
    "BKB" => ("Nal Airport","Bikaner","T1"),
    "JSA" => ("Jaisalmer Airport","Jaisalmer","T1"),
    "AGR" => ("Agra Airport","Agra","T1"),
    "GWL" => ("Gwalior Airport","Gwalior","T1"),
    "JLR" => ("Jabalpur Airport","Jabalpur","T1"),
    _     => (code + " Airport", code, "T1")
};

// ── Route dataset — 120 real IndiGo domestic routes ──────────────────────
var routes = new (string from, string to, int durMins, int econ, int biz)[]
{
    // ── Metro ↔ Metro ────────────────────────────────────────────────────
    ("DEL","BOM",130,3499,7499),("BOM","DEL",130,3599,7599),
    ("DEL","BLR",150,3799,7999),("BLR","DEL",150,3899,7999),
    ("DEL","HYD",140,3199,6999),("HYD","DEL",140,3299,6999),
    ("DEL","MAA",160,3599,7799),("MAA","DEL",160,3699,7899),
    ("DEL","CCU",140,3299,7199),("CCU","DEL",140,3399,7299),
    ("BOM","BLR",90,2499,5999),("BLR","BOM",90,2599,5999),
    ("BOM","HYD",90,2199,5499),("HYD","BOM",90,2299,5499),
    ("BOM","MAA",120,2899,6799),("MAA","BOM",120,2999,6799),
    ("BOM","CCU",150,3399,7399),("CCU","BOM",150,3499,7399),
    ("BLR","HYD",60,1499,3999),("HYD","BLR",60,1599,3999),
    ("BLR","MAA",60,1199,3499),("MAA","BLR",60,1299,3499),
    ("BLR","CCU",145,3199,7099),("CCU","BLR",145,3299,7099),
    ("HYD","MAA",75,1599,4199),("MAA","HYD",75,1699,4199),
    ("HYD","CCU",135,2999,6899),("CCU","HYD",135,3099,6899),
    ("MAA","CCU",130,2899,6799),("CCU","MAA",130,2999,6799),

    // ── Metro ↔ Goa ──────────────────────────────────────────────────────
    ("BOM","GOI",75,1799,4499),("GOI","BOM",75,1899,4499),
    ("DEL","GOI",140,3299,7099),("GOI","DEL",140,3399,7099),
    ("BLR","GOI",80,1699,4399),("GOI","BLR",80,1799,4399),
    ("HYD","GOI",85,1899,4699),("GOI","HYD",85,1999,4699),
    ("MAA","GOI",95,2099,5199),("GOI","MAA",95,2199,5199),

    // ── North India ──────────────────────────────────────────────────────
    ("DEL","AMD",90,2799,6499),("AMD","DEL",90,2899,6499),
    ("DEL","PNQ",120,2999,6799),("PNQ","DEL",120,3099,6799),
    ("DEL","JAI",60,1399,3799),("JAI","DEL",60,1499,3799),
    ("DEL","LKO",75,1899,4799),("LKO","DEL",75,1999,4799),
    ("DEL","IXC",60,1799,4499),("IXC","DEL",60,1899,4499),
    ("DEL","ATQ",90,2099,5399),("ATQ","DEL",90,2199,5399),
    ("DEL","SXR",80,1999,5199),("SXR","DEL",80,2099,5199),
    ("DEL","IXL",105,2399,5899),("IXL","DEL",105,2499,5899),
    ("DEL","VNS",70,1699,4299),("VNS","DEL",70,1799,4299),
    ("DEL","PAT",95,2149,5449),("PAT","DEL",95,2249,5449),
    ("DEL","AGR",45,1299,3499),("AGR","DEL",45,1399,3499),
    ("DEL","GWL",55,1499,3899),("GWL","DEL",55,1599,3899),
    ("DEL","UDR",95,2199,5599),("UDR","DEL",95,2299,5599),
    ("DEL","JDH",75,1849,4649),("JDH","DEL",75,1949,4649),
    ("DEL","BDQ",120,2699,6399),("BDQ","DEL",120,2799,6399),

    // ── Northeast ────────────────────────────────────────────────────────
    ("DEL","GAU",130,2899,6699),("GAU","DEL",130,2999,6699),
    ("DEL","IXB",155,3199,7099),("IXB","DEL",155,3299,7099),
    ("DEL","DIB",165,3399,7499),("DIB","DEL",165,3499,7499),
    ("DEL","AGX",160,3299,7299),("AGX","DEL",160,3399,7299),
    ("CCU","GAU",60,1599,4099),("GAU","CCU",60,1699,4099),
    ("CCU","IXB",60,1699,4299),("IXB","CCU",60,1799,4299),
    ("CCU","IMF",75,1899,4699),("IMF","CCU",75,1999,4699),
    ("CCU","BBI",65,1599,4099),("BBI","CCU",65,1699,4099),

    // ── West India ───────────────────────────────────────────────────────
    ("BOM","PNQ",30,999,2999),("PNQ","BOM",30,1099,2999),
    ("BOM","AMD",70,1599,4199),("AMD","BOM",70,1699,4199),
    ("BOM","NAG",70,1699,4299),("NAG","BOM",70,1799,4299),
    ("BOM","IDR",90,2099,5299),("IDR","BOM",90,2199,5299),
    ("BOM","BHO",70,1699,4299),("BHO","BOM",70,1799,4299),
    ("BOM","IXR",120,2599,6299),("IXR","BOM",120,2699,6299),
    ("BOM","UDR",95,2199,5599),("UDR","BOM",95,2299,5599),
    ("AMD","BLR",110,2399,5999),("BLR","AMD",110,2499,5999),
    ("AMD","HYD",100,2249,5649),("HYD","AMD",100,2349,5649),
    ("AMD","PNQ",75,1699,4299),("PNQ","AMD",75,1799,4299),
    ("AMD","JAI",70,1649,4149),("JAI","AMD",70,1749,4149),
    ("AMD","GOI",85,1949,4849),("GOI","AMD",85,2049,4849),
    ("STV","DEL",110,2449,5949),("DEL","STV",110,2349,5949),
    ("BDQ","BOM",70,1649,4149),("BOM","BDQ",70,1749,4149),

    // ── South India ──────────────────────────────────────────────────────
    ("BLR","COK",75,1799,4599),("COK","BLR",75,1899,4599),
    ("BLR","TRV",85,1899,4699),("TRV","BLR",85,1999,4699),
    ("BLR","CJB",55,1299,3599),("CJB","BLR",55,1399,3599),
    ("BLR","IXM",95,2099,5199),("IXM","BLR",95,2199,5199),
    ("BLR","IXE",55,1299,3599),("IXE","BLR",55,1399,3599),
    ("BLR","VTZ",100,2199,5499),("VTZ","BLR",100,2299,5499),
    ("MAA","COK",70,1499,3999),("COK","MAA",70,1599,3999),
    ("MAA","TRV",90,1949,4849),("TRV","MAA",90,2049,4849),
    ("MAA","TRZ",60,1299,3599),("TRZ","MAA",60,1399,3599),
    ("MAA","CJB",65,1399,3799),("CJB","MAA",65,1499,3799),
    ("HYD","COK",95,2049,5149),("COK","HYD",95,2149,5149),
    ("HYD","VGA",55,1299,3599),("VGA","HYD",55,1399,3599),
    ("HYD","TIR",75,1799,4499),("TIR","HYD",75,1899,4499),
    ("HYD","VTZ",80,1849,4649),("VTZ","HYD",80,1949,4649),
    ("HYD","RJA",60,1399,3799),("RJA","HYD",60,1499,3799),

    // ── Central India ────────────────────────────────────────────────────
    ("DEL","NAG",110,2449,5949),("NAG","DEL",110,2549,5949),
    ("DEL","BHO",80,1899,4699),("BHO","DEL",80,1999,4699),
    ("DEL","IDR",90,2099,5299),("IDR","DEL",90,2199,5299),
    ("DEL","RPR",105,2349,5849),("RPR","DEL",105,2449,5849),
    ("DEL","JLR",95,2149,5449),("JLR","DEL",95,2249,5449),
    ("DEL","HJR",70,1699,4299),("HJR","DEL",70,1799,4299),
    ("BOM","RPR",115,2549,6149),("RPR","BOM",115,2649,6149),
    ("HYD","NAG",55,1399,3699),("NAG","HYD",55,1499,3699),
    ("BLR","HBX",75,1799,4499),("HBX","BLR",75,1899,4499),

    // ── Rajasthan ────────────────────────────────────────────────────────
    ("JAI","BOM",85,1999,4999),("BOM","JAI",85,2099,4999),
    ("JAI","BLR",125,2699,6499),("BLR","JAI",125,2799,6499),
    ("JAI","HYD",120,2649,6349),("HYD","JAI",120,2749,6349),

    // ── UP / Bihar ───────────────────────────────────────────────────────
    ("LKO","BOM",120,2649,6349),("BOM","LKO",120,2749,6349),
    ("LKO","BLR",140,2949,6849),("BLR","LKO",140,3049,6849),
    ("LKO","HYD",130,2849,6649),("HYD","LKO",130,2949,6649),
    ("PAT","BOM",130,2849,6649),("BOM","PAT",130,2949,6649),
    ("VNS","BOM",120,2649,6349),("BOM","VNS",120,2749,6349),
    ("GOP","DEL",75,1849,4649),("DEL","GOP",75,1949,4649),
};

Console.WriteLine($"Total routes to seed: {routes.Length}");

var docs = routes.Select((r, i) =>
{
    var (f, t, dur, econ, biz) = r;
    var dep = AP(f); var arr = AP(t);
    int h = 6 + (i * 7 % 16); int m = (i * 13) % 60;
    int ah = (h + dur / 60) % 24; int am = (m + dur % 60) % 60;
    int booked = rnd.Next(5, 176);
    return new BsonDocument
    {
        {"flightId",       $"6E-{1001 + i}"},
        {"flightNumber",   $"6E-{1001 + i}"},
        {"airline",        "IndiGo"},
        {"departure", new BsonDocument {
            {"code",f},{"name",dep.name},{"city",dep.city},{"terminal",dep.term}}},
        {"arrival", new BsonDocument {
            {"code",t},{"name",arr.name},{"city",arr.city},{"terminal",arr.term}}},
        {"departureTime",  $"{h:D2}:{m:D2}"},
        {"arrivalTime",    $"{ah:D2}:{am:D2}"},
        {"durationMinutes", dur},
        {"aircraft",       i % 5 == 0 ? "Airbus A321" : "Airbus A320"},
        {"totalSeats",     186},
        {"bookedSeats",    booked},
        {"availableSeats", 186 - booked},
        {"fare", new BsonDocument {{"economy", econ}, {"business", biz}}},
        {"status", i % 12 == 0 ? "Delayed" : "On Time"},
        {"lastUpdated", DateTime.UtcNow}
    };
}).ToList();

await col.InsertManyAsync(docs);
Console.WriteLine($"✓ Seeded {docs.Count} IndiGo routes into '{dbName}.flights'");
Console.WriteLine("✓ Open MongoDB Compass → mongodb://localhost:27017 to browse the data.");
