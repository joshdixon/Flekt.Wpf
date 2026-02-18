using Flekt.Wpf.Shared;

namespace Flekt.Wpf.Api;

public class ChaosState
{
    private readonly Lock _lock = new();
    private ChaosProfile _profile = new();

    public ChaosProfile Profile
    {
        get { lock (_lock) return _profile; }
        set { lock (_lock) _profile = value; }
    }
}

public class ChaosMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context, ChaosState chaosState)
    {
        var profile = chaosState.Profile;
        var path = context.Request.Path.Value?.ToLowerInvariant() ?? "";

        var delay = path switch
        {
            "/api/auth/login" => profile.LoginDelayMs,
            "/api/stock" or "/api/stock/search" => profile.StockLoadDelayMs,
            "/api/stock/adjust" => profile.StockAdjustDelayMs,
            "/api/invoices/run" => profile.InvoiceProcessDelayMs,
            _ when path.StartsWith("/api/dockets") && path.EndsWith("/allocate") => profile.DocketAllocateDelayMs,
            _ when path.StartsWith("/api/dockets") && path.EndsWith("/confirm") => profile.DocketConfirmDelayMs,
            _ when path.StartsWith("/api/dockets") => profile.DocketLoadDelayMs,
            _ => 0
        };

        if (delay > 0)
            await Task.Delay(delay, context.RequestAborted);

        await next(context);
    }
}
