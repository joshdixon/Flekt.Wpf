using System.Text.Json;
using Flekt.Wpf.Api;
using Flekt.Wpf.Api.Data;
using Flekt.Wpf.Shared;
using Flekt.Wpf.Shared.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
    options.AddDefaultPolicy(policy =>
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));

// Load chaos config from file
var configPath = Environment.GetEnvironmentVariable("CHAOS_CONFIG_PATH")
    ?? Path.Combine(Directory.GetCurrentDirectory(), "configs", "baseline.json");

var chaosState = new ChaosState();
if (File.Exists(configPath))
{
    var json = await File.ReadAllTextAsync(configPath);
    var profile = JsonSerializer.Deserialize<ChaosProfile>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
    if (profile is not null)
        chaosState.Profile = profile;
}

builder.Services.AddSingleton(chaosState);

var app = builder.Build();

app.UseCors();
app.UseMiddleware<ChaosMiddleware>();

// In-memory data store
var stockItems = SeedData.GetStockItems();
var dockets = SeedData.GetDockets(stockItems);
var nextDocketId = dockets.Max(d => d.Id) + 1;
var nextLineId = dockets.SelectMany(d => d.Lines).Max(l => l.Id) + 1;
var loginAttempts = new Dictionary<string, int>();

// Health check
app.MapGet("/api/health", () => Results.Ok(new { status = "healthy", timestamp = DateTime.UtcNow }));

// Auth
app.MapPost("/api/auth/login", (LoginRequest request, ChaosState chaos) =>
{
    var key = request.Username?.ToLowerInvariant() ?? "";
    var profile = chaos.Profile;

    // Track login attempts for chaos "fail first attempt" feature
    if (profile.LoginFailsFirstAttempt)
    {
        loginAttempts.TryGetValue(key, out var attempts);
        loginAttempts[key] = attempts + 1;

        if (attempts == 0)
        {
            return Results.Unauthorized();
        }
    }

    // Accept any non-empty credentials
    if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
        return Results.BadRequest(new { error = "Username and password are required" });

    return Results.Ok(new LoginResponse
    {
        Token = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes($"{request.Username}:{DateTime.UtcNow:O}")),
        DisplayName = request.Username
    });
});

// Stock
app.MapGet("/api/stock", (ChaosState chaos) =>
{
    var profile = chaos.Profile;
    var items = stockItems.ToList();

    // Chaos: wrong quantities (multiply by random factor)
    if (profile.MissingStockColumn)
    {
        // Return items without Location field (set to empty)
        items = items.Select(i => new StockItem
        {
            Id = i.Id, Sku = i.Sku, Name = i.Name, Category = i.Category,
            Quantity = i.Quantity, Location = "", UnitPrice = i.UnitPrice, LastUpdated = i.LastUpdated
        }).ToList();
    }

    return Results.Ok(items);
});

app.MapGet("/api/stock/search", (string? q, ChaosState chaos) =>
{
    if (string.IsNullOrWhiteSpace(q))
        return Results.Ok(stockItems);

    var filtered = stockItems.Where(i =>
        i.Sku.Contains(q, StringComparison.OrdinalIgnoreCase) ||
        i.Name.Contains(q, StringComparison.OrdinalIgnoreCase) ||
        i.Category.Contains(q, StringComparison.OrdinalIgnoreCase) ||
        i.Location.Contains(q, StringComparison.OrdinalIgnoreCase)
    ).ToList();

    return Results.Ok(filtered);
});

// Invoices
app.MapPost("/api/invoices/run", (ChaosState chaos) =>
{
    var profile = chaos.Profile;

    var groups = stockItems
        .GroupBy(i => i.Category)
        .Select(g => new InvoiceCategoryGroup
        {
            Category = g.Key,
            ItemCount = g.Count(),
            TotalQty = g.Sum(i => i.Quantity),
            TotalValue = g.Sum(i => i.Quantity * i.UnitPrice),
            AvgUnitPrice = g.Average(i => i.UnitPrice)
        })
        .OrderBy(g => g.Category)
        .ToList();

    // Chaos: wrong totals
    if (profile.WrongInvoiceTotals)
    {
        foreach (var g in groups)
            g.TotalValue *= 1.15m; // Inflate by 15%
    }

    // Chaos: phantom row
    if (profile.PhantomInvoiceRow)
    {
        groups.Add(new InvoiceCategoryGroup
        {
            Category = "Miscellaneous",
            ItemCount = 3,
            TotalQty = 47,
            TotalValue = 892.50m,
            AvgUnitPrice = 19.00m
        });
    }

    var summary = new InvoiceSummary
    {
        Groups = groups,
        TotalItemCount = groups.Sum(g => g.ItemCount),
        TotalQuantity = groups.Sum(g => g.TotalQty),
        TotalValue = groups.Sum(g => g.TotalValue),
        OverallAvgUnitPrice = groups.Average(g => g.AvgUnitPrice)
    };

    return Results.Ok(summary);
});

// Dockets
app.MapGet("/api/dockets", (string? type) =>
{
    var items = dockets.AsEnumerable();
    if (Enum.TryParse<DocketType>(type, true, out var docketType))
        items = items.Where(d => d.Type == docketType);
    return Results.Ok(items.OrderByDescending(d => d.CreatedAt).ToList());
});

app.MapGet("/api/dockets/{id:int}", (int id) =>
{
    var docket = dockets.FirstOrDefault(d => d.Id == id);
    return docket is null ? Results.NotFound() : Results.Ok(docket);
});

app.MapPost("/api/dockets", (CreateDocketRequest request) =>
{
    var docket = new Docket
    {
        Id = nextDocketId++,
        Type = request.Type,
        Status = DocketStatus.Draft,
        Sender = request.Sender,
        Receiver = request.Receiver,
        CreatedAt = DateTime.UtcNow,
        Lines = request.Lines.Select(l => new DocketLine
        {
            Id = nextLineId++,
            Sku = l.Sku,
            Name = l.Name,
            QuantityRequested = l.QuantityRequested,
            Location = l.Location
        }).ToList()
    };
    dockets.Add(docket);
    return Results.Created($"/api/dockets/{docket.Id}", docket);
});

app.MapPost("/api/dockets/{id:int}/allocate", (int id, AllocateDocketRequest request) =>
{
    var docket = dockets.FirstOrDefault(d => d.Id == id);
    if (docket is null) return Results.NotFound();
    if (docket.Status != DocketStatus.Draft)
        return Results.BadRequest(new { error = "Only Draft dockets can be allocated" });

    foreach (var allocLine in request.Lines)
    {
        var line = docket.Lines.FirstOrDefault(l => l.Id == allocLine.LineId);
        if (line is null) continue;
        line.Location = allocLine.Location;
        line.AllocatedQty = allocLine.AllocatedQty;

        // For pick dockets, validate stock exists at location
        if (docket.Type == DocketType.Pick)
        {
            var stock = stockItems.FirstOrDefault(s =>
                s.Sku.Equals(line.Sku, StringComparison.OrdinalIgnoreCase));
            if (stock is null || stock.Quantity < allocLine.AllocatedQty)
                return Results.BadRequest(new { error = $"Insufficient stock for {line.Sku}" });
        }
    }

    docket.Status = DocketStatus.Allocated;
    return Results.Ok(docket);
});

app.MapPost("/api/dockets/{id:int}/confirm", (int id) =>
{
    var docket = dockets.FirstOrDefault(d => d.Id == id);
    if (docket is null) return Results.NotFound();
    if (docket.Status != DocketStatus.Allocated)
        return Results.BadRequest(new { error = "Only Allocated dockets can be confirmed" });

    foreach (var line in docket.Lines)
    {
        var stock = stockItems.FirstOrDefault(s =>
            s.Sku.Equals(line.Sku, StringComparison.OrdinalIgnoreCase));

        if (docket.Type == DocketType.Pick)
        {
            // Pick decrements stock
            if (stock is not null)
            {
                stock.Quantity -= line.AllocatedQty ?? line.QuantityRequested;
                stock.LastUpdated = DateTime.UtcNow;
            }
        }
        else
        {
            // Put increments stock (or creates new item)
            if (stock is not null)
            {
                stock.Quantity += line.AllocatedQty ?? line.QuantityRequested;
                stock.LastUpdated = DateTime.UtcNow;
            }
            else
            {
                stockItems.Add(new StockItem
                {
                    Id = stockItems.Max(s => s.Id) + 1,
                    Sku = line.Sku,
                    Name = line.Name,
                    Category = "Uncategorised",
                    Quantity = line.AllocatedQty ?? line.QuantityRequested,
                    Location = line.Location ?? "",
                    UnitPrice = 0m,
                    LastUpdated = DateTime.UtcNow
                });
            }
        }
    }

    docket.Status = DocketStatus.Confirmed;
    return Results.Ok(docket);
});

app.MapPost("/api/dockets/{id:int}/cancel", (int id) =>
{
    var docket = dockets.FirstOrDefault(d => d.Id == id);
    if (docket is null) return Results.NotFound();
    if (docket.Status is DocketStatus.Confirmed or DocketStatus.Cancelled)
        return Results.BadRequest(new { error = "Cannot cancel a Confirmed or already Cancelled docket" });

    docket.Status = DocketStatus.Cancelled;
    return Results.Ok(docket);
});

// Stock adjustment
app.MapPost("/api/stock/adjust", (StockAdjustmentRequest request) =>
{
    var stock = stockItems.FirstOrDefault(s => s.Id == request.StockItemId);
    if (stock is null) return Results.NotFound();

    var previous = stock.Quantity;
    stock.Quantity += request.AdjustmentQuantity;
    if (stock.Quantity < 0) stock.Quantity = 0;
    stock.LastUpdated = DateTime.UtcNow;

    return Results.Ok(new StockAdjustmentResponse
    {
        StockItemId = stock.Id,
        PreviousQuantity = previous,
        NewQuantity = stock.Quantity
    });
});

// Chaos config management
app.MapGet("/api/chaos", (ChaosState chaos) => Results.Ok(chaos.Profile));

app.MapPut("/api/chaos", async (HttpContext context, ChaosState chaos) =>
{
    var profile = await context.Request.ReadFromJsonAsync<ChaosProfile>();
    if (profile is null)
        return Results.BadRequest(new { error = "Invalid chaos profile" });

    chaos.Profile = profile;
    return Results.Ok(chaos.Profile);
});

app.Run();
