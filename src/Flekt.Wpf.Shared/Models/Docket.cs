namespace Flekt.Wpf.Shared.Models;

public enum DocketType
{
    Pick,
    Put
}

public enum DocketStatus
{
    Draft,
    Allocated,
    Confirmed,
    Cancelled
}

public class Docket
{
    public int Id { get; set; }
    public DocketType Type { get; set; }
    public DocketStatus Status { get; set; }
    public string Sender { get; set; } = string.Empty;
    public string Receiver { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public List<DocketLine> Lines { get; set; } = [];
}

public class DocketLine
{
    public int Id { get; set; }
    public string Sku { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public int QuantityRequested { get; set; }
    public string? Location { get; set; }
    public int? AllocatedQty { get; set; }
}
