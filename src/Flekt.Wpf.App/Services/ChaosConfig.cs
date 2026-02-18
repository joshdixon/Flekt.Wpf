using System.IO;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using Flekt.Wpf.Shared;

namespace Flekt.Wpf.App.Services;

public class ChaosConfig
{
    private ChaosProfile _profile = new();
    private readonly JsonSerializerOptions _jsonOptions = new() { PropertyNameCaseInsensitive = true };

    public ChaosProfile Profile => _profile;

    public async Task LoadFromFileAsync(string path)
    {
        if (!File.Exists(path)) return;
        var json = await File.ReadAllTextAsync(path);
        var profile = JsonSerializer.Deserialize<ChaosProfile>(json, _jsonOptions);
        if (profile is not null)
            _profile = profile;
    }

    public async Task LoadFromApiAsync(HttpClient client)
    {
        var response = await client.GetAsync("/api/chaos");
        if (response.IsSuccessStatusCode)
        {
            var profile = await response.Content.ReadFromJsonAsync<ChaosProfile>(_jsonOptions);
            if (profile is not null)
                _profile = profile;
        }
    }

    public void Update(ChaosProfile profile) => _profile = profile;
}
