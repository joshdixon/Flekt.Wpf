using System.Net.Http;
using System.Net.Http.Json;
using Flekt.Wpf.Shared;
using Flekt.Wpf.Shared.Models;

namespace Flekt.Wpf.App.Services;

public class ApiClient
{
    private readonly HttpClient _client;
    private string? _token;

    public ApiClient(string baseUrl)
    {
        _client = new HttpClient { BaseAddress = new Uri(baseUrl) };
    }

    public bool IsAuthenticated => _token is not null;

    public async Task<LoginResponse> LoginAsync(string username, string password)
    {
        var response = await _client.PostAsJsonAsync("/api/auth/login", new LoginRequest
        {
            Username = username,
            Password = password
        });

        if (!response.IsSuccessStatusCode)
            throw new HttpRequestException($"Login failed: {response.StatusCode}");

        var result = await response.Content.ReadFromJsonAsync<LoginResponse>()
            ?? throw new InvalidOperationException("Invalid login response");

        _token = result.Token;
        _client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _token);

        return result;
    }

    public async Task<List<StockItem>> GetStockAsync()
    {
        var response = await _client.GetAsync("/api/stock");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<List<StockItem>>() ?? [];
    }

    public async Task<List<StockItem>> SearchStockAsync(string query)
    {
        var response = await _client.GetAsync($"/api/stock/search?q={Uri.EscapeDataString(query)}");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<List<StockItem>>() ?? [];
    }

    public async Task<InvoiceSummary> RunInvoicesAsync()
    {
        var response = await _client.PostAsync("/api/invoices/run", null);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<InvoiceSummary>()
            ?? throw new InvalidOperationException("Invalid invoice response");
    }

    public async Task<ChaosProfile> GetChaosProfileAsync()
    {
        var response = await _client.GetAsync("/api/chaos");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<ChaosProfile>() ?? new ChaosProfile();
    }

    public async Task<List<Docket>> GetDocketsAsync(DocketType? type = null)
    {
        var url = type.HasValue ? $"/api/dockets?type={type.Value}" : "/api/dockets";
        var response = await _client.GetAsync(url);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<List<Docket>>() ?? [];
    }

    public async Task<Docket> GetDocketAsync(int id)
    {
        var response = await _client.GetAsync($"/api/dockets/{id}");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<Docket>()
            ?? throw new InvalidOperationException("Invalid docket response");
    }

    public async Task<Docket> CreateDocketAsync(CreateDocketRequest request)
    {
        var response = await _client.PostAsJsonAsync("/api/dockets", request);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<Docket>()
            ?? throw new InvalidOperationException("Invalid docket response");
    }

    public async Task<Docket> AllocateDocketAsync(int id, AllocateDocketRequest request)
    {
        var response = await _client.PostAsJsonAsync($"/api/dockets/{id}/allocate", request);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<Docket>()
            ?? throw new InvalidOperationException("Invalid docket response");
    }

    public async Task<Docket> ConfirmDocketAsync(int id)
    {
        var response = await _client.PostAsync($"/api/dockets/{id}/confirm", null);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<Docket>()
            ?? throw new InvalidOperationException("Invalid docket response");
    }

    public async Task<Docket> CancelDocketAsync(int id)
    {
        var response = await _client.PostAsync($"/api/dockets/{id}/cancel", null);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<Docket>()
            ?? throw new InvalidOperationException("Invalid docket response");
    }

    public async Task<StockAdjustmentResponse> AdjustStockAsync(StockAdjustmentRequest request)
    {
        var response = await _client.PostAsJsonAsync("/api/stock/adjust", request);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<StockAdjustmentResponse>()
            ?? throw new InvalidOperationException("Invalid stock adjustment response");
    }
}
