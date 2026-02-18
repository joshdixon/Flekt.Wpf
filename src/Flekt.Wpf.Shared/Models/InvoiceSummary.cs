namespace Flekt.Wpf.Shared.Models;

public class InvoiceSummary
{
    public List<InvoiceCategoryGroup> Groups { get; set; } = [];
    public int TotalItemCount { get; set; }
    public int TotalQuantity { get; set; }
    public decimal TotalValue { get; set; }
    public decimal OverallAvgUnitPrice { get; set; }
}

public class InvoiceCategoryGroup
{
    public string Category { get; set; } = string.Empty;
    public int ItemCount { get; set; }
    public int TotalQty { get; set; }
    public decimal TotalValue { get; set; }
    public decimal AvgUnitPrice { get; set; }
}
