using Bookstore.Translation;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace Bookstore
{
    public partial class SingInUp : Window
    {
        public SingInUp()
        {
            InitializeComponent();
        }

        private async void Login_Click(object sender, RoutedEventArgs e)
        {
            var username = LoginUsername.Text;
            var password = LoginPassword.Password;

            try
            {
                if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                {
                    LoginResult.Text = "Podaj login i hasło.";
                    return;
                }
                bool success = await App.Auth.LoginAsync(username, password);
                if (!success)
                {
                    LoginResult.Text = "Nieprawidłowy login lub hasło.";
                    return;
                }
                bool isAdmin = await App.Auth.IsAdminAsync();
                var mainWindow = new MainWindow(isAdmin);
                mainWindow.Show();
                this.Close();
            }
            catch (Exception ex)
            {
                LoginResult.Text = $"Błąd: {ex.Message}";
                LoginResult.Foreground = Brushes.Red;
            }
        }

        private async void Register_Click(object sender, RoutedEventArgs e)
        {
            var email = RegEmail.Text;
            var username = RegUsername.Text;
            var firstName = RegFirstName.Text;
            var lastName = RegLastName.Text;
            var password = RegPassword.Password;

            try
            {
                if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(username) ||
                               string.IsNullOrEmpty(firstName) || string.IsNullOrEmpty(lastName) ||
                               string.IsNullOrEmpty(password))
                {
                    RegisterResult.Text = "Wypełnij wszystkie pola.";
                    return;
                }

                bool success = await App.Auth.RegisterAsync(email, username, firstName, lastName, password);
                if (!success)
                {
                    RegisterResult.Text = "Rejestracja nie powiodła się (login lub email już istnieje).";
                    return;
                }

                // Po rejestracji user jest już zalogowany. Sprawdź rolę i idź do MainWindow.
                bool isAdmin = await App.Auth.IsAdminAsync();
                var mainWindow = new MainWindow(isAdmin);
                mainWindow.Show();
                this.Close();
            }
            catch (Exception ex)
            {
                RegisterResult.Text = $"Błąd: {ex.Message}";
                RegisterResult.Foreground = Brushes.Red;
            }
        }
        private void LanguageToggle_Checked(object sender, RoutedEventArgs e)
        {
            LocalizationManager.ChangeLanguage("en");
        }

        private void LanguageToggle_Unchecked(object sender, RoutedEventArgs e)
        {
            LocalizationManager.ChangeLanguage("pl");
        }
    }
}
