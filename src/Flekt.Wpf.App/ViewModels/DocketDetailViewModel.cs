using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Flekt.Wpf.App.Services;
using Flekt.Wpf.Shared.Models;

namespace Flekt.Wpf.App.ViewModels;

public partial class DocketDetailViewModel : ObservableObject
{
    private readonly ApiClient _apiClient;
    private readonly ChaosConfig _chaosConfig;
    private readonly Action _onBack;

    [ObservableProperty] private Docket _docket;
    [ObservableProperty] private ObservableCollection<DocketLine> _lines = [];
    [ObservableProperty] private bool _isLoading;
    [ObservableProperty] private string _statusText = string.Empty;

    public bool CanAllocate => Docket.Status == DocketStatus.Draft;
    public bool CanConfirm => Docket.Status == DocketStatus.Allocated;
    public bool CanCancel => Docket.Status is DocketStatus.Draft or DocketStatus.Allocated;
    public bool IsEditable => Docket.Status == DocketStatus.Draft;

    public DocketDetailViewModel(ApiClient apiClient, ChaosConfig chaosConfig, Docket docket, Action onBack)
    {
        _apiClient = apiClient;
        _chaosConfig = chaosConfig;
        _docket = docket;
        _onBack = onBack;
        Lines = new ObservableCollection<DocketLine>(docket.Lines);
    }

    [RelayCommand]
    private void GoBack() => _onBack();

    [RelayCommand]
    private async Task AllocateAsync()
    {
        IsLoading = true;
        StatusText = "Allocating...";
        try
        {
            var delay = _chaosConfig.Profile.DocketAllocateDelayMs;
            if (delay > 0)
                await Task.Delay(delay);

            var request = new AllocateDocketRequest
            {
                Lines = Lines.Select(l => new AllocateDocketLineRequest
                {
                    LineId = l.Id,
                    Location = l.Location,
                    AllocatedQty = l.QuantityRequested
                }).ToList()
            };

            var updated = await _apiClient.AllocateDocketAsync(Docket.Id, request);
            UpdateFromDocket(updated);
            StatusText = "Docket allocated successfully";
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
    private async Task ConfirmAsync()
    {
        IsLoading = true;
        StatusText = "Confirming...";
        try
        {
            var delay = _chaosConfig.Profile.DocketConfirmDelayMs;
            if (delay > 0)
                await Task.Delay(delay);

            var updated = await _apiClient.ConfirmDocketAsync(Docket.Id);
            UpdateFromDocket(updated);
            StatusText = "Docket confirmed — stock updated";
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
    private async Task CancelAsync()
    {
        IsLoading = true;
        StatusText = "Cancelling...";
        try
        {
            var updated = await _apiClient.CancelDocketAsync(Docket.Id);
            UpdateFromDocket(updated);
            StatusText = "Docket cancelled";
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

    private void UpdateFromDocket(Docket updated)
    {
        Docket = updated;
        Lines = new ObservableCollection<DocketLine>(updated.Lines);
        OnPropertyChanged(nameof(CanAllocate));
        OnPropertyChanged(nameof(CanConfirm));
        OnPropertyChanged(nameof(CanCancel));
        OnPropertyChanged(nameof(IsEditable));
    }
}
