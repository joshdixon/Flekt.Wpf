using System.IO;
using System.Text.Json;
using System.Windows;
using Flekt.Wpf.App.Services;
using Flekt.Wpf.App.ViewModels;
using Flekt.Wpf.App.Views;
using Flekt.Wpf.Shared;

namespace Flekt.Wpf.App;

public partial class App : Application
{
    private ChaosConfig _chaosConfig = new();
    private ApiClient _apiClient = null!;

    private async void Application_Startup(object sender, StartupEventArgs e)
    {
        // Load settings
        var apiUrl = "http://localhost:5100";
        var configPath = "configs/baseline.json";

        // Check for appsettings.json
        var settingsPath = Path.Combine(AppContext.BaseDirectory, "appsettings.json");
        if (File.Exists(settingsPath))
        {
            var settings = JsonSerializer.Deserialize<JsonElement>(await File.ReadAllTextAsync(settingsPath));
            if (settings.TryGetProperty("ApiUrl", out var urlProp))
                apiUrl = urlProp.GetString() ?? apiUrl;
            if (settings.TryGetProperty("ChaosConfigPath", out var cfgProp))
                configPath = cfgProp.GetString() ?? configPath;
        }

        // Override from command line args
        foreach (var arg in e.Args)
        {
            if (arg.StartsWith("--api="))
                apiUrl = arg["--api=".Length..];
            else if (arg.StartsWith("--chaos="))
                configPath = arg["--chaos=".Length..];
        }

        _apiClient = new ApiClient(apiUrl);
        _chaosConfig = new ChaosConfig();

        // Try to load chaos config from file first, then from API
        if (File.Exists(configPath))
        {
            await _chaosConfig.LoadFromFileAsync(configPath);
        }
        else
        {
            try { await _chaosConfig.LoadFromApiAsync(new System.Net.Http.HttpClient { BaseAddress = new Uri(apiUrl) }); }
            catch { /* Use defaults */ }
        }

        // Show random error dialog if chaos says so
        if (_chaosConfig.Profile.ShowRandomErrorDialog)
        {
            MessageBox.Show(
                "An unexpected error occurred while initializing the application.\nError code: 0xDEADBEEF",
                "Application Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }

        ShowLogin();
    }

    private void ShowLogin()
    {
        var loginVm = new LoginViewModel(_apiClient, _chaosConfig, OnLoginSuccess);
        var loginView = new LoginView { DataContext = loginVm };
        loginView.SetupPasswordBinding();

        var loginWindow = new Window
        {
            Title = "Flekt Warehouse - Sign In",
            Width = 450,
            Height = 500,
            WindowStartupLocation = WindowStartupLocation.CenterScreen,
            ResizeMode = ResizeMode.NoResize,
            Content = loginView,
            Resources = Current.Resources
        };

        loginWindow.Show();
    }

    private void OnLoginSuccess(string displayName)
    {
        var mainVm = new MainViewModel(_apiClient, _chaosConfig, displayName);
        var mainWindow = new MainWindow();
        mainWindow.Initialize(mainVm, _apiClient, _chaosConfig);
        mainWindow.Show();

        // Close the login window
        foreach (Window window in Windows)
        {
            if (window != mainWindow)
                window.Close();
        }
    }
}
