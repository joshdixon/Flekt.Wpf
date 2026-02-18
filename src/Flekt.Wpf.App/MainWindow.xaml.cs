using System.ComponentModel;
using System.Windows;
using Flekt.Wpf.App.Services;
using Flekt.Wpf.App.ViewModels;
using Flekt.Wpf.App.Views;
using Flekt.Wpf.Shared.Models;

namespace Flekt.Wpf.App;

public partial class MainWindow : Window
{
    private MainViewModel? _viewModel;
    private ApiClient? _apiClient;
    private ChaosConfig? _chaosConfig;
    private StockView? _stockView;
    private InvoiceView? _invoiceView;
    private DocketListView? _pickDocketsView;
    private DocketListView? _putDocketsView;
    private DocketDetailView? _docketDetailView;

    public MainWindow()
    {
        InitializeComponent();
    }

    public void Initialize(MainViewModel viewModel, ApiClient apiClient, ChaosConfig chaosConfig)
    {
        _viewModel = viewModel;
        _apiClient = apiClient;
        _chaosConfig = chaosConfig;
        DataContext = viewModel;

        _stockView = new StockView { DataContext = viewModel.StockVm };
        _invoiceView = new InvoiceView { DataContext = viewModel.InvoiceVm };
        _pickDocketsView = new DocketListView { DataContext = viewModel.PickDocketsVm };
        _putDocketsView = new DocketListView { DataContext = viewModel.PutDocketsVm };

        viewModel.StockVm.AdjustStockRequested += OnAdjustStockRequested;
        viewModel.PropertyChanged += ViewModel_PropertyChanged;

        // Show stock view by default
        ShowStockView();
    }

    private void ViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(MainViewModel.CurrentViewName))
        {
            switch (_viewModel!.CurrentViewName)
            {
                case "Stock":
                    ShowStockView();
                    break;
                case "Invoices":
                    ShowInvoiceView();
                    break;
                case "PickDockets":
                    ShowPickDocketsView();
                    break;
                case "PutDockets":
                    ShowPutDocketsView();
                    break;
                case "DocketDetail":
                    ShowDocketDetailView();
                    break;
            }
        }
    }

    private void ShowStockView()
    {
        ContentArea.Content = _stockView;
        _ = _viewModel!.StockVm.LoadStockCommand.ExecuteAsync(null);
    }

    private void ShowInvoiceView()
    {
        ContentArea.Content = _invoiceView;
    }

    private void ShowPickDocketsView()
    {
        ContentArea.Content = _pickDocketsView;
        _ = _viewModel!.PickDocketsVm.LoadDocketsCommand.ExecuteAsync(null);
    }

    private void ShowPutDocketsView()
    {
        ContentArea.Content = _putDocketsView;
        _ = _viewModel!.PutDocketsVm.LoadDocketsCommand.ExecuteAsync(null);
    }

    private void ShowDocketDetailView()
    {
        _docketDetailView = new DocketDetailView { DataContext = _viewModel!.DocketDetailVm };
        ContentArea.Content = _docketDetailView;
    }

    private void OnAdjustStockRequested(StockItem stockItem)
    {
        var vm = new StockAdjustmentViewModel(_apiClient!, _chaosConfig!, stockItem, success =>
        {
            Dispatcher.Invoke(() =>
            {
                // Close the dialog — find it from owned windows
                foreach (Window w in OwnedWindows)
                {
                    if (w is StockAdjustmentDialog)
                    {
                        w.Close();
                        break;
                    }
                }

                if (success)
                    _ = _viewModel!.StockVm.LoadStockCommand.ExecuteAsync(null);
            });
        });

        var dialog = new StockAdjustmentDialog
        {
            DataContext = vm,
            Owner = this
        };
        dialog.ShowDialog();
    }

    private void About_Click(object sender, RoutedEventArgs e)
    {
        MessageBox.Show(
            "Flekt Warehouse Management System\nVersion 1.0.0\n\nA test fixture for Flekt validation.",
            "About",
            MessageBoxButton.OK,
            MessageBoxImage.Information);
    }
}
