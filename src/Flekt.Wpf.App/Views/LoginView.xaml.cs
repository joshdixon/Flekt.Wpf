using System.Windows;
using System.Windows.Controls;
using Flekt.Wpf.App.ViewModels;

namespace Flekt.Wpf.App.Views;

public partial class LoginView : UserControl
{
    public LoginView()
    {
        InitializeComponent();
    }

    // Wire up PasswordBox (can't bind directly due to security)
    public void SetupPasswordBinding()
    {
        PasswordBox.PasswordChanged += (_, _) =>
        {
            if (DataContext is LoginViewModel vm)
                vm.Password = PasswordBox.Password;
        };
    }
}
