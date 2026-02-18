namespace Flekt.Wpf.Shared.Models;

public class StockItem
{
    public int Id { get; set; }
    public string Sku { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public string Location { get; set; } = string.Empty;
    public decimal UnitPrice { get; set; }
    public DateTime LastUpdated { get; set; }
}
