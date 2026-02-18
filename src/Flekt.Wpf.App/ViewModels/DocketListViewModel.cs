using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Flekt.Wpf.App.Services;
using Flekt.Wpf.Shared.Models;

namespace Flekt.Wpf.App.ViewModels;

public partial class DocketListViewModel : ObservableObject
{
    private readonly ApiClient _apiClient;
    private readonly ChaosConfig _chaosConfig;
    private readonly DocketType _docketType;
    private readonly Action<Docket> _onDocketSelected;

    [ObservableProperty] private ObservableCollection<Docket> _dockets = [];
    [ObservableProperty] private bool _isLoading;
    [ObservableProperty] private string _statusText = string.Empty;

    public string Title => _docketType == DocketType.Pick ? "Pick Dockets" : "Put Dockets";

    public DocketListViewModel(ApiClient apiClient, ChaosConfig chaosConfig, DocketType docketType, Action<Docket> onDocketSelected)
    {
        _apiClient = apiClient;
        _chaosConfig = chaosConfig;
        _docketType = docketType;
        _onDocketSelected = onDocketSelected;
    }

    [RelayCommand]
    public async Task LoadDocketsAsync()
    {
        IsLoading = true;
        StatusText = "Loading dockets...";
        try
        {
            var delay = _chaosConfig.Profile.DocketLoadDelayMs;
            if (delay > 0)
                await Task.Delay(delay);

            var items = await _apiClient.GetDocketsAsync(_docketType);
            Dockets = new ObservableCollection<Docket>(items);
            StatusText = $"{items.Count} dockets";
        }
        catch (Exception ex)
        {
            StatusText = $"Error: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private void SelectDocket(Docket? docket)
    {
        if (docket is not null)
            _onDocketSelected(docket);
    }
}
