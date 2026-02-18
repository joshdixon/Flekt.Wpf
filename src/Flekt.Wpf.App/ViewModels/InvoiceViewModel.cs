using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Flekt.Wpf.App.Services;
using Flekt.Wpf.Shared.Models;

namespace Flekt.Wpf.App.ViewModels;

public partial class InvoiceViewModel : ObservableObject
{
    private readonly ApiClient _apiClient;
    private readonly ChaosConfig _chaosConfig;

    [ObservableProperty] private ObservableCollection<InvoiceCategoryGroup> _invoiceGroups = [];
    [ObservableProperty] private bool _isProcessing;
    [ObservableProperty] private bool _hasResults;
    [ObservableProperty] private int _totalItemCount;
    [ObservableProperty] private int _totalQuantity;
    [ObservableProperty] private decimal _totalValue;
    [ObservableProperty] private decimal _overallAvgUnitPrice;
    [ObservableProperty] private string _statusText = "Click 'Run Invoice Process' to generate summary.";
    [ObservableProperty] private int _progressPercent;

    public InvoiceViewModel(ApiClient apiClient, ChaosConfig chaosConfig)
    {
        _apiClient = apiClient;
        _chaosConfig = chaosConfig;
    }

    [RelayCommand]
    private async Task RunInvoicesAsync()
    {
        IsProcessing = true;
        HasResults = false;
        StatusText = "Processing invoices...";
        ProgressPercent = 0;

        try
        {
            // Simulate progress with client-side delay
            var totalDelay = _chaosConfig.Profile.InvoiceProcessDelayMs;
            var steps = 10;
            var stepDelay = totalDelay / steps;

            for (int i = 1; i <= steps; i++)
            {
                await Task.Delay(Math.Max(stepDelay, 50));
                ProgressPercent = i * 10;
            }

            var summary = await _apiClient.RunInvoicesAsync();

            InvoiceGroups = new ObservableCollection<InvoiceCategoryGroup>(summary.Groups);
            TotalItemCount = summary.TotalItemCount;
            TotalQuantity = summary.TotalQuantity;
            TotalValue = summary.TotalValue;
            OverallAvgUnitPrice = summary.OverallAvgUnitPrice;
            HasResults = true;
            StatusText = $"Invoice processing complete. {summary.Groups.Count} categories found.";
        }
        catch (Exception ex)
        {
            StatusText = $"Error: {ex.Message}";
        }
        finally
        {
            IsProcessing = false;
        }
    }
}
