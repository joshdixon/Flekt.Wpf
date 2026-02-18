namespace Flekt.Wpf.Shared.Models;

public class CreateDocketRequest
{
    public DocketType Type { get; set; }
    public string Sender { get; set; } = string.Empty;
    public string Receiver { get; set; } = string.Empty;
    public List<CreateDocketLineRequest> Lines { get; set; } = [];
}

public class CreateDocketLineRequest
{
    public string Sku { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public int QuantityRequested { get; set; }
    public string? Location { get; set; }
}

public class AllocateDocketRequest
{
    public List<AllocateDocketLineRequest> Lines { get; set; } = [];
}

public class AllocateDocketLineRequest
{
    public int LineId { get; set; }
    public string? Location { get; set; }
    public int AllocatedQty { get; set; }
}
