using Flekt.Wpf.Shared.Models;

namespace Flekt.Wpf.Api.Data;

public static class SeedData
{
    public static List<Docket> GetDockets(List<StockItem> stockItems)
    {
        var dockets = new List<Docket>();
        var id = 1;
        var lineId = 1;

        // Draft Pick
        dockets.Add(new Docket
        {
            Id = id++, Type = DocketType.Pick, Status = DocketStatus.Draft,
            Sender = "Warehouse A", Receiver = "Customer - Acme Corp",
            CreatedAt = DateTime.UtcNow.AddHours(-4),
            Lines =
            [
                new() { Id = lineId++, Sku = "WH-BOX-001", Name = "Small Cardboard Box", QuantityRequested = 50 },
                new() { Id = lineId++, Sku = "WH-TPE-001", Name = "Packing Tape (48mm)", QuantityRequested = 10 },
                new() { Id = lineId++, Sku = "WH-LBL-001", Name = "Shipping Labels (500pk)", QuantityRequested = 2 }
            ]
        });

        // Draft Put
        dockets.Add(new Docket
        {
            Id = id++, Type = DocketType.Put, Status = DocketStatus.Draft,
            Sender = "Supplier - BoxCo Ltd", Receiver = "Warehouse B",
            CreatedAt = DateTime.UtcNow.AddHours(-3),
            Lines =
            [
                new() { Id = lineId++, Sku = "WH-PLT-001", Name = "Standard Pallet", QuantityRequested = 100 },
                new() { Id = lineId++, Sku = "WH-WRP-001", Name = "Bubble Wrap Roll", QuantityRequested = 30 }
            ]
        });

        // Allocated Pick
        dockets.Add(new Docket
        {
            Id = id++, Type = DocketType.Pick, Status = DocketStatus.Allocated,
            Sender = "Warehouse A", Receiver = "Customer - BuildRight",
            CreatedAt = DateTime.UtcNow.AddHours(-8),
            Lines =
            [
                new() { Id = lineId++, Sku = "WH-SAF-001", Name = "Safety Vest (Hi-Vis)", QuantityRequested = 10, Location = "E-01-01", AllocatedQty = 10 },
                new() { Id = lineId++, Sku = "WH-SAF-002", Name = "Hard Hat", QuantityRequested = 10, Location = "E-01-02", AllocatedQty = 10 }
            ]
        });

        // Allocated Put
        dockets.Add(new Docket
        {
            Id = id++, Type = DocketType.Put, Status = DocketStatus.Allocated,
            Sender = "Supplier - SafetyFirst", Receiver = "Warehouse A",
            CreatedAt = DateTime.UtcNow.AddHours(-6),
            Lines =
            [
                new() { Id = lineId++, Sku = "WH-SAF-003", Name = "Steel Toe Boots (Pair)", QuantityRequested = 20, Location = "E-02-01", AllocatedQty = 20 },
                new() { Id = lineId++, Sku = "WH-SAF-004", Name = "Safety Goggles", QuantityRequested = 40, Location = "E-02-02", AllocatedQty = 40 }
            ]
        });

        // Confirmed Pick
        dockets.Add(new Docket
        {
            Id = id++, Type = DocketType.Pick, Status = DocketStatus.Confirmed,
            Sender = "Warehouse B", Receiver = "Customer - LogiTrans",
            CreatedAt = DateTime.UtcNow.AddDays(-1),
            Lines =
            [
                new() { Id = lineId++, Sku = "WH-BOX-002", Name = "Medium Cardboard Box", QuantityRequested = 200, Location = "B-02-02", AllocatedQty = 200 },
                new() { Id = lineId++, Sku = "WH-FIL-001", Name = "Void Fill Peanuts (Bag)", QuantityRequested = 50, Location = "B-06-01", AllocatedQty = 50 }
            ]
        });

        // Confirmed Put
        dockets.Add(new Docket
        {
            Id = id++, Type = DocketType.Put, Status = DocketStatus.Confirmed,
            Sender = "Supplier - PackAll", Receiver = "Warehouse A",
            CreatedAt = DateTime.UtcNow.AddDays(-2),
            Lines =
            [
                new() { Id = lineId++, Sku = "WH-BOX-003", Name = "Large Cardboard Box", QuantityRequested = 100, Location = "B-02-03", AllocatedQty = 100 },
                new() { Id = lineId++, Sku = "WH-WRP-002", Name = "Stretch Wrap Film", QuantityRequested = 50, Location = "B-04-02", AllocatedQty = 50 }
            ]
        });

        return dockets;
    }

    public static List<StockItem> GetStockItems() =>
    [
        new() { Id = 1, Sku = "WH-PLT-001", Name = "Standard Pallet", Category = "Pallets", Quantity = 342, Location = "A-01-01", UnitPrice = 25.00m, LastUpdated = DateTime.UtcNow.AddHours(-2) },
        new() { Id = 2, Sku = "WH-PLT-002", Name = "Euro Pallet", Category = "Pallets", Quantity = 187, Location = "A-01-02", UnitPrice = 32.50m, LastUpdated = DateTime.UtcNow.AddHours(-5) },
        new() { Id = 3, Sku = "WH-PLT-003", Name = "Heavy Duty Pallet", Category = "Pallets", Quantity = 94, Location = "A-01-03", UnitPrice = 45.00m, LastUpdated = DateTime.UtcNow.AddDays(-1) },
        new() { Id = 4, Sku = "WH-BOX-001", Name = "Small Cardboard Box", Category = "Packaging", Quantity = 1250, Location = "B-02-01", UnitPrice = 1.20m, LastUpdated = DateTime.UtcNow.AddHours(-1) },
        new() { Id = 5, Sku = "WH-BOX-002", Name = "Medium Cardboard Box", Category = "Packaging", Quantity = 890, Location = "B-02-02", UnitPrice = 2.50m, LastUpdated = DateTime.UtcNow.AddHours(-3) },
        new() { Id = 6, Sku = "WH-BOX-003", Name = "Large Cardboard Box", Category = "Packaging", Quantity = 456, Location = "B-02-03", UnitPrice = 4.75m, LastUpdated = DateTime.UtcNow.AddHours(-8) },
        new() { Id = 7, Sku = "WH-BOX-004", Name = "Insulated Box", Category = "Packaging", Quantity = 120, Location = "B-03-01", UnitPrice = 8.90m, LastUpdated = DateTime.UtcNow.AddDays(-2) },
        new() { Id = 8, Sku = "WH-WRP-001", Name = "Bubble Wrap Roll", Category = "Packaging", Quantity = 75, Location = "B-04-01", UnitPrice = 15.00m, LastUpdated = DateTime.UtcNow.AddHours(-4) },
        new() { Id = 9, Sku = "WH-WRP-002", Name = "Stretch Wrap Film", Category = "Packaging", Quantity = 200, Location = "B-04-02", UnitPrice = 12.30m, LastUpdated = DateTime.UtcNow.AddDays(-1) },
        new() { Id = 10, Sku = "WH-LBL-001", Name = "Shipping Labels (500pk)", Category = "Labels", Quantity = 340, Location = "C-01-01", UnitPrice = 18.50m, LastUpdated = DateTime.UtcNow.AddHours(-6) },
        new() { Id = 11, Sku = "WH-LBL-002", Name = "Barcode Labels (1000pk)", Category = "Labels", Quantity = 560, Location = "C-01-02", UnitPrice = 22.00m, LastUpdated = DateTime.UtcNow.AddHours(-12) },
        new() { Id = 12, Sku = "WH-LBL-003", Name = "Hazmat Warning Labels", Category = "Labels", Quantity = 180, Location = "C-01-03", UnitPrice = 35.00m, LastUpdated = DateTime.UtcNow.AddDays(-3) },
        new() { Id = 13, Sku = "WH-TLS-001", Name = "Pallet Jack Manual", Category = "Equipment", Quantity = 8, Location = "D-01-01", UnitPrice = 350.00m, LastUpdated = DateTime.UtcNow.AddDays(-10) },
        new() { Id = 14, Sku = "WH-TLS-002", Name = "Electric Pallet Jack", Category = "Equipment", Quantity = 4, Location = "D-01-02", UnitPrice = 2800.00m, LastUpdated = DateTime.UtcNow.AddDays(-30) },
        new() { Id = 15, Sku = "WH-TLS-003", Name = "Hand Truck", Category = "Equipment", Quantity = 12, Location = "D-02-01", UnitPrice = 120.00m, LastUpdated = DateTime.UtcNow.AddDays(-5) },
        new() { Id = 16, Sku = "WH-TLS-004", Name = "Platform Cart", Category = "Equipment", Quantity = 6, Location = "D-02-02", UnitPrice = 250.00m, LastUpdated = DateTime.UtcNow.AddDays(-7) },
        new() { Id = 17, Sku = "WH-TLS-005", Name = "Barcode Scanner", Category = "Equipment", Quantity = 15, Location = "D-03-01", UnitPrice = 450.00m, LastUpdated = DateTime.UtcNow.AddDays(-2) },
        new() { Id = 18, Sku = "WH-SAF-001", Name = "Safety Vest (Hi-Vis)", Category = "Safety", Quantity = 50, Location = "E-01-01", UnitPrice = 14.00m, LastUpdated = DateTime.UtcNow.AddHours(-24) },
        new() { Id = 19, Sku = "WH-SAF-002", Name = "Hard Hat", Category = "Safety", Quantity = 35, Location = "E-01-02", UnitPrice = 22.00m, LastUpdated = DateTime.UtcNow.AddDays(-4) },
        new() { Id = 20, Sku = "WH-SAF-003", Name = "Steel Toe Boots (Pair)", Category = "Safety", Quantity = 28, Location = "E-02-01", UnitPrice = 85.00m, LastUpdated = DateTime.UtcNow.AddDays(-15) },
        new() { Id = 21, Sku = "WH-SAF-004", Name = "Safety Goggles", Category = "Safety", Quantity = 60, Location = "E-02-02", UnitPrice = 9.50m, LastUpdated = DateTime.UtcNow.AddDays(-6) },
        new() { Id = 22, Sku = "WH-SAF-005", Name = "Ear Plugs (100pk)", Category = "Safety", Quantity = 40, Location = "E-03-01", UnitPrice = 12.00m, LastUpdated = DateTime.UtcNow.AddDays(-8) },
        new() { Id = 23, Sku = "WH-CLN-001", Name = "Floor Cleaner (5L)", Category = "Maintenance", Quantity = 24, Location = "F-01-01", UnitPrice = 18.00m, LastUpdated = DateTime.UtcNow.AddDays(-3) },
        new() { Id = 24, Sku = "WH-CLN-002", Name = "Degreaser Spray", Category = "Maintenance", Quantity = 36, Location = "F-01-02", UnitPrice = 8.50m, LastUpdated = DateTime.UtcNow.AddDays(-2) },
        new() { Id = 25, Sku = "WH-CLN-003", Name = "Industrial Mop", Category = "Maintenance", Quantity = 10, Location = "F-02-01", UnitPrice = 45.00m, LastUpdated = DateTime.UtcNow.AddDays(-20) },
        new() { Id = 26, Sku = "WH-SHV-001", Name = "Steel Shelving Unit", Category = "Storage", Quantity = 18, Location = "G-01-01", UnitPrice = 180.00m, LastUpdated = DateTime.UtcNow.AddDays(-14) },
        new() { Id = 27, Sku = "WH-SHV-002", Name = "Wire Shelving Unit", Category = "Storage", Quantity = 22, Location = "G-01-02", UnitPrice = 120.00m, LastUpdated = DateTime.UtcNow.AddDays(-9) },
        new() { Id = 28, Sku = "WH-BIN-001", Name = "Plastic Storage Bin (S)", Category = "Storage", Quantity = 200, Location = "G-02-01", UnitPrice = 5.50m, LastUpdated = DateTime.UtcNow.AddHours(-18) },
        new() { Id = 29, Sku = "WH-BIN-002", Name = "Plastic Storage Bin (M)", Category = "Storage", Quantity = 150, Location = "G-02-02", UnitPrice = 9.00m, LastUpdated = DateTime.UtcNow.AddHours(-36) },
        new() { Id = 30, Sku = "WH-BIN-003", Name = "Plastic Storage Bin (L)", Category = "Storage", Quantity = 80, Location = "G-02-03", UnitPrice = 14.50m, LastUpdated = DateTime.UtcNow.AddDays(-4) },
        new() { Id = 31, Sku = "WH-TPE-001", Name = "Packing Tape (48mm)", Category = "Packaging", Quantity = 500, Location = "B-05-01", UnitPrice = 3.20m, LastUpdated = DateTime.UtcNow.AddHours(-7) },
        new() { Id = 32, Sku = "WH-TPE-002", Name = "Fragile Tape Roll", Category = "Packaging", Quantity = 150, Location = "B-05-02", UnitPrice = 5.00m, LastUpdated = DateTime.UtcNow.AddDays(-1) },
        new() { Id = 33, Sku = "WH-FIL-001", Name = "Void Fill Peanuts (Bag)", Category = "Packaging", Quantity = 300, Location = "B-06-01", UnitPrice = 6.80m, LastUpdated = DateTime.UtcNow.AddDays(-2) },
        new() { Id = 34, Sku = "WH-FIL-002", Name = "Air Pillow Roll", Category = "Packaging", Quantity = 45, Location = "B-06-02", UnitPrice = 28.00m, LastUpdated = DateTime.UtcNow.AddDays(-5) },
        new() { Id = 35, Sku = "WH-DOC-001", Name = "Packing Slip Pouch (500pk)", Category = "Labels", Quantity = 80, Location = "C-02-01", UnitPrice = 25.00m, LastUpdated = DateTime.UtcNow.AddDays(-3) },
        new() { Id = 36, Sku = "WH-RCK-001", Name = "Pallet Racking Bay", Category = "Storage", Quantity = 30, Location = "G-03-01", UnitPrice = 320.00m, LastUpdated = DateTime.UtcNow.AddDays(-60) },
        new() { Id = 37, Sku = "WH-RCK-002", Name = "Cantilever Rack", Category = "Storage", Quantity = 8, Location = "G-03-02", UnitPrice = 550.00m, LastUpdated = DateTime.UtcNow.AddDays(-45) },
        new() { Id = 38, Sku = "WH-SIG-001", Name = "Aisle Marker Sign", Category = "Safety", Quantity = 100, Location = "E-04-01", UnitPrice = 7.50m, LastUpdated = DateTime.UtcNow.AddDays(-12) },
        new() { Id = 39, Sku = "WH-SIG-002", Name = "Fire Exit Sign", Category = "Safety", Quantity = 20, Location = "E-04-02", UnitPrice = 15.00m, LastUpdated = DateTime.UtcNow.AddDays(-30) },
        new() { Id = 40, Sku = "WH-SIG-003", Name = "Speed Limit Sign (5mph)", Category = "Safety", Quantity = 12, Location = "E-04-03", UnitPrice = 18.00m, LastUpdated = DateTime.UtcNow.AddDays(-25) },
        new() { Id = 41, Sku = "WH-CHG-001", Name = "Forklift Battery Charger", Category = "Equipment", Quantity = 3, Location = "D-04-01", UnitPrice = 1200.00m, LastUpdated = DateTime.UtcNow.AddDays(-90) },
        new() { Id = 42, Sku = "WH-CHG-002", Name = "Barcode Scanner Cradle", Category = "Equipment", Quantity = 10, Location = "D-03-02", UnitPrice = 85.00m, LastUpdated = DateTime.UtcNow.AddDays(-7) },
        new() { Id = 43, Sku = "WH-OFC-001", Name = "Printer Paper (A4 Ream)", Category = "Office", Quantity = 150, Location = "H-01-01", UnitPrice = 5.50m, LastUpdated = DateTime.UtcNow.AddDays(-1) },
        new() { Id = 44, Sku = "WH-OFC-002", Name = "Receipt Printer Roll", Category = "Office", Quantity = 200, Location = "H-01-02", UnitPrice = 3.00m, LastUpdated = DateTime.UtcNow.AddHours(-10) },
        new() { Id = 45, Sku = "WH-OFC-003", Name = "Clipboard", Category = "Office", Quantity = 30, Location = "H-02-01", UnitPrice = 4.50m, LastUpdated = DateTime.UtcNow.AddDays(-14) },
        new() { Id = 46, Sku = "WH-OFC-004", Name = "Pen Box (50pk)", Category = "Office", Quantity = 25, Location = "H-02-02", UnitPrice = 12.00m, LastUpdated = DateTime.UtcNow.AddDays(-6) },
        new() { Id = 47, Sku = "WH-MAT-001", Name = "Anti-Fatigue Floor Mat", Category = "Safety", Quantity = 15, Location = "E-05-01", UnitPrice = 45.00m, LastUpdated = DateTime.UtcNow.AddDays(-20) },
        new() { Id = 48, Sku = "WH-MAT-002", Name = "Rubber Dock Bumper", Category = "Equipment", Quantity = 20, Location = "D-05-01", UnitPrice = 65.00m, LastUpdated = DateTime.UtcNow.AddDays(-35) },
        new() { Id = 49, Sku = "WH-LGT-001", Name = "LED Bay Light", Category = "Maintenance", Quantity = 40, Location = "F-03-01", UnitPrice = 55.00m, LastUpdated = DateTime.UtcNow.AddDays(-50) },
        new() { Id = 50, Sku = "WH-LGT-002", Name = "Emergency Exit Light", Category = "Maintenance", Quantity = 16, Location = "F-03-02", UnitPrice = 38.00m, LastUpdated = DateTime.UtcNow.AddDays(-40) },
    ];
}
