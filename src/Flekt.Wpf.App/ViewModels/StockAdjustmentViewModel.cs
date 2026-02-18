using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Flekt.Wpf.App.Services;
using Flekt.Wpf.Shared.Models;

namespace Flekt.Wpf.App.ViewModels;

public partial class StockAdjustmentViewModel : ObservableObject
{
    private readonly ApiClient _apiClient;
    private readonly ChaosConfig _chaosConfig;
    private readonly Action<bool> _onClose;

    [ObservableProperty] private string _sku = string.Empty;
    [ObservableProperty] private string _name = string.Empty;
    [ObservableProperty] private string _location = string.Empty;
    [ObservableProperty] private int _currentQuantity;
    [ObservableProperty] private int _adjustmentQuantity;
    [ObservableProperty] private string _reason = string.Empty;
    [ObservableProperty] private bool _isSubmitting;
    [ObservableProperty] private string _errorMessage = string.Empty;

    public int StockItemId { get; }

    public StockAdjustmentViewModel(ApiClient apiClient, ChaosConfig chaosConfig, StockItem stockItem, Action<bool> onClose)
    {
        _apiClient = apiClient;
        _chaosConfig = chaosConfig;
        _onClose = onClose;
        StockItemId = stockItem.Id;
        Sku = stockItem.Sku;
        Name = stockItem.Name;
        Location = stockItem.Location;
        CurrentQuantity = stockItem.Quantity;
    }

    [RelayCommand]
    private async Task SubmitAsync()
    {
        if (AdjustmentQuantity == 0)
        {
            ErrorMessage = "Adjustment quantity cannot be zero";
            return;
        }

        IsSubmitting = true;
        ErrorMessage = string.Empty;
        try
        {
            var delay = _chaosConfig.Profile.StockAdjustDelayMs;
            if (delay > 0)
                await Task.Delay(delay);

            await _apiClient.AdjustStockAsync(new StockAdjustmentRequest
            {
                StockItemId = StockItemId,
                AdjustmentQuantity = AdjustmentQuantity,
                Reason = Reason
            });

            _onClose(true);
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Error: {ex.Message}";
        }
        finally
        {
            IsSubmitting = false;
        }
    }

    [RelayCommand]
    private void Cancel() => _onClose(false);
}
