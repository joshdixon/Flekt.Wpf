using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Flekt.Wpf.App.Services;
using Flekt.Wpf.Shared.Models;

namespace Flekt.Wpf.App.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private readonly ApiClient _apiClient;
    private readonly ChaosConfig _chaosConfig;

    [ObservableProperty] private object? _currentView;
    [ObservableProperty] private string _currentUser = string.Empty;
    [ObservableProperty] private string _statusText = "Connected";
    [ObservableProperty] private string _currentViewName = "Stock";
    [ObservableProperty] private bool _isNavigating;

    // View models (lazy-created)
    private StockViewModel? _stockVm;
    private InvoiceViewModel? _invoiceVm;
    private DocketListViewModel? _pickDocketsVm;
    private DocketListViewModel? _putDocketsVm;
    private DocketDetailViewModel? _docketDetailVm;

    public MainViewModel(ApiClient apiClient, ChaosConfig chaosConfig, string displayName)
    {
        _apiClient = apiClient;
        _chaosConfig = chaosConfig;
        CurrentUser = displayName;
    }

    public StockViewModel StockVm => _stockVm ??= new StockViewModel(_apiClient, _chaosConfig);
    public InvoiceViewModel InvoiceVm => _invoiceVm ??= new InvoiceViewModel(_apiClient, _chaosConfig);
    public DocketListViewModel PickDocketsVm => _pickDocketsVm ??= new DocketListViewModel(_apiClient, _chaosConfig, DocketType.Pick, OnDocketSelected);
    public DocketListViewModel PutDocketsVm => _putDocketsVm ??= new DocketListViewModel(_apiClient, _chaosConfig, DocketType.Put, OnDocketSelected);
    public DocketDetailViewModel? DocketDetailVm => _docketDetailVm;

    [RelayCommand]
    private async Task NavigateToStockAsync()
    {
        await ApplyNavigationDelay();
        CurrentViewName = "Stock";
        OnPropertyChanged(nameof(CurrentViewName));
    }

    [RelayCommand]
    private async Task NavigateToInvoicesAsync()
    {
        await ApplyNavigationDelay();
        CurrentViewName = "Invoices";
        OnPropertyChanged(nameof(CurrentViewName));
    }

    [RelayCommand]
    private async Task NavigateToPickDocketsAsync()
    {
        await ApplyNavigationDelay();
        CurrentViewName = "PickDockets";
        OnPropertyChanged(nameof(CurrentViewName));
    }

    [RelayCommand]
    private async Task NavigateToPutDocketsAsync()
    {
        await ApplyNavigationDelay();
        CurrentViewName = "PutDockets";
        OnPropertyChanged(nameof(CurrentViewName));
    }

    private void OnDocketSelected(Docket docket)
    {
        _docketDetailVm = new DocketDetailViewModel(_apiClient, _chaosConfig, docket, OnDocketDetailBack);
        OnPropertyChanged(nameof(DocketDetailVm));
        CurrentViewName = "DocketDetail";
        OnPropertyChanged(nameof(CurrentViewName));
    }

    private void OnDocketDetailBack()
    {
        // Return to the appropriate docket list
        var wasPickType = _docketDetailVm?.Docket.Type == DocketType.Pick;
        _docketDetailVm = null;
        CurrentViewName = wasPickType ? "PickDockets" : "PutDockets";
        OnPropertyChanged(nameof(CurrentViewName));
    }

    private async Task ApplyNavigationDelay()
    {
        var delay = _chaosConfig.Profile.NavigationDelayMs;
        if (delay > 0)
        {
            IsNavigating = true;
            await Task.Delay(delay);
            IsNavigating = false;
        }
    }
}
