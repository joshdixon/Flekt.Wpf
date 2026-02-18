namespace Flekt.Wpf.Shared.Models;

public class StockAdjustmentRequest
{
    public int StockItemId { get; set; }
    public int AdjustmentQuantity { get; set; }
    public string Reason { get; set; } = string.Empty;
}

public class StockAdjustmentResponse
{
    public int StockItemId { get; set; }
    public int PreviousQuantity { get; set; }
    public int NewQuantity { get; set; }
}
