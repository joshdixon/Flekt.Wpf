namespace Flekt.Wpf.Shared;

public class ChaosProfile
{
    // Delays (milliseconds) - applied both client and server side
    public int LoginDelayMs { get; set; } = 200;
    public int StockLoadDelayMs { get; set; } = 500;
    public int InvoiceProcessDelayMs { get; set; } = 1500;
    public int NavigationDelayMs { get; set; } = 100;
    public int DocketLoadDelayMs { get; set; } = 300;
    public int DocketAllocateDelayMs { get; set; } = 500;
    public int DocketConfirmDelayMs { get; set; } = 400;
    public int StockAdjustDelayMs { get; set; } = 200;

    // Bugs
    public bool LoginFailsFirstAttempt { get; set; }
    public bool WrongInvoiceTotals { get; set; }
    public bool MissingStockColumn { get; set; }
    public bool ShiftedLoginButton { get; set; }
    public bool ShowRandomErrorDialog { get; set; }
    public bool PhantomInvoiceRow { get; set; }

    // Visual changes
    public string Theme { get; set; } = "default"; // default | dark | highcontrast
    public double FontScale { get; set; } = 1.0;
    public string AccentColor { get; set; } = "#0078D4";
}
