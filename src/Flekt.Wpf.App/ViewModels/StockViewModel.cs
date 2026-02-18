using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Flekt.Wpf.App.Services;
using Flekt.Wpf.Shared.Models;

namespace Flekt.Wpf.App.ViewModels;

public partial class StockViewModel : ObservableObject
{
    private readonly ApiClient _apiClient;
    private readonly ChaosConfig _chaosConfig;
    private List<StockItem> _allItems = [];

    [ObservableProperty] private ObservableCollection<StockItem> _stockItems = [];
    [ObservableProperty] private string _searchText = string.Empty;
    [ObservableProperty] private bool _isLoading;
    [ObservableProperty] private bool _showLocationColumn = true;
    [ObservableProperty] private string _statusText = string.Empty;
    [ObservableProperty] private StockItem? _selectedStockItem;

    public event Action<StockItem>? AdjustStockRequested;

    // Column filters
    [ObservableProperty] private string _filterSku = string.Empty;
    [ObservableProperty] private string _filterName = string.Empty;
    [ObservableProperty] private string _filterCategory = string.Empty;
    [ObservableProperty] private string _filterLocation = string.Empty;

    // Grouping
    [ObservableProperty] private string _groupByColumn = "None";

    // Available categories for filter dropdown
    [ObservableProperty] private ObservableCollection<string> _availableCategories = [""];

    // Available group-by options
    public List<string> GroupByOptions { get; } = ["None", "Category", "Location"];

    private ICollectionView? _stockView;
    public ICollectionView? StockView
    {
        get => _stockView;
        private set => SetProperty(ref _stockView, value);
    }

    public StockViewModel(ApiClient apiClient, ChaosConfig chaosConfig)
    {
        _apiClient = apiClient;
        _chaosConfig = chaosConfig;
        ShowLocationColumn = !chaosConfig.Profile.MissingStockColumn;
    }

    partial void OnFilterSkuChanged(string value) => ApplyFilter();
    partial void OnFilterNameChanged(string value) => ApplyFilter();
    partial void OnFilterCategoryChanged(string value) => ApplyFilter();
    partial void OnFilterLocationChanged(string value) => ApplyFilter();

    partial void OnGroupByColumnChanged(string value) => ApplyGrouping();

    [RelayCommand]
    public async Task LoadStockAsync()
    {
        IsLoading = true;
        StatusText = "Loading stock data...";
        try
        {
            var delay = _chaosConfig.Profile.StockLoadDelayMs;
            if (delay > 0)
                await Task.Delay(delay);

            _allItems = string.IsNullOrWhiteSpace(SearchText)
                ? await _apiClient.GetStockAsync()
                : await _apiClient.SearchStockAsync(SearchText);

            StockItems = new ObservableCollection<StockItem>(_allItems);
            StockView = CollectionViewSource.GetDefaultView(StockItems);
            StockView.Filter = FilterPredicate;

            // Populate category dropdown
            var categories = _allItems.Select(i => i.Category).Distinct().OrderBy(c => c).ToList();
            categories.Insert(0, "");
            AvailableCategories = new ObservableCollection<string>(categories);

            ApplyGrouping();
            UpdateStatusText();
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
    private async Task SearchAsync()
    {
        await LoadStockAsync();
    }

    [RelayCommand]
    private void AdjustStock()
    {
        if (SelectedStockItem is not null)
            AdjustStockRequested?.Invoke(SelectedStockItem);
    }

    [RelayCommand]
    private void ClearFilters()
    {
        FilterSku = string.Empty;
        FilterName = string.Empty;
        FilterCategory = string.Empty;
        FilterLocation = string.Empty;
    }

    private bool FilterPredicate(object obj)
    {
        if (obj is not StockItem item) return false;

        if (!string.IsNullOrEmpty(FilterSku) &&
            !item.Sku.Contains(FilterSku, StringComparison.OrdinalIgnoreCase))
            return false;

        if (!string.IsNullOrEmpty(FilterName) &&
            !item.Name.Contains(FilterName, StringComparison.OrdinalIgnoreCase))
            return false;

        if (!string.IsNullOrEmpty(FilterCategory) &&
            !item.Category.Equals(FilterCategory, StringComparison.OrdinalIgnoreCase))
            return false;

        if (!string.IsNullOrEmpty(FilterLocation) &&
            !item.Location.Contains(FilterLocation, StringComparison.OrdinalIgnoreCase))
            return false;

        return true;
    }

    private void ApplyFilter()
    {
        StockView?.Refresh();
        UpdateStatusText();
    }

    private void ApplyGrouping()
    {
        if (StockView is null) return;

        StockView.GroupDescriptions.Clear();

        if (GroupByColumn is not "None")
            StockView.GroupDescriptions.Add(new PropertyGroupDescription(GroupByColumn));
    }

    private void UpdateStatusText()
    {
        if (StockView is null)
        {
            StatusText = $"{_allItems.Count} items loaded";
            return;
        }

        var visibleCount = StockView.Cast<object>().Count();
        StatusText = visibleCount == _allItems.Count
            ? $"{_allItems.Count} items"
            : $"{visibleCount} of {_allItems.Count} items (filtered)";
    }
}
