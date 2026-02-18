using System.Net.Http;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Flekt.Wpf.App.Services;

namespace Flekt.Wpf.App.ViewModels;

public partial class LoginViewModel : ObservableObject
{
    private readonly ApiClient _apiClient;
    private readonly ChaosConfig _chaosConfig;
    private readonly Action<string> _onLoginSuccess;

    [ObservableProperty] private string _username = string.Empty;
    [ObservableProperty] private string _password = string.Empty;
    [ObservableProperty] private bool _isLoading;
    [ObservableProperty] private string _errorMessage = string.Empty;
    [ObservableProperty] private bool _hasError;
    [ObservableProperty] private Thickness _loginButtonMargin = new(0);

    public LoginViewModel(ApiClient apiClient, ChaosConfig chaosConfig, Action<string> onLoginSuccess)
    {
        _apiClient = apiClient;
        _chaosConfig = chaosConfig;
        _onLoginSuccess = onLoginSuccess;

        // Chaos: shifted login button
        if (_chaosConfig.Profile.ShiftedLoginButton)
            LoginButtonMargin = new Thickness(80, 0, 0, 0);
    }

    [RelayCommand]
    private async Task LoginAsync()
    {
        HasError = false;
        ErrorMessage = string.Empty;

        if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
        {
            ErrorMessage = "Please enter username and password.";
            HasError = true;
            return;
        }

        IsLoading = true;
        try
        {
            // Client-side chaos delay
            var delay = _chaosConfig.Profile.LoginDelayMs;
            if (delay > 0)
                await Task.Delay(delay);

            var response = await _apiClient.LoginAsync(Username, Password);
            _onLoginSuccess(response.DisplayName);
        }
        catch (HttpRequestException ex) when (ex.Message.Contains("Unauthorized"))
        {
            ErrorMessage = "Invalid username or password. Please try again.";
            HasError = true;
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Connection error: {ex.Message}";
            HasError = true;
        }
        finally
        {
            IsLoading = false;
        }
    }
}
