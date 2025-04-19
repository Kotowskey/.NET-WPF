using Bookstore.SignalRHub;
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
        private ConnectionHub _connection;
        public SingInUp(ConnectionHub connectionHub)
        {
            InitializeComponent();
            _connection = connectionHub;
        }

        private async void Login_Click(object sender, RoutedEventArgs e)
        {
            var username = LoginUsername.Text;
            var password = LoginPassword.Password;

            try
            {
                var success = await _connection.Login(username, password);
                if (success)
                {
                    LoginResult.Text = "Zalogowano!";
                    LoginResult.Foreground = Brushes.Green;

                    var main = new MainWindow(_connection);
                    main.Show();
                    this.Close();
                }
                else
                {
                    LoginResult.Text = "Niepoprawny login lub hasło.";
                    LoginResult.Foreground = Brushes.Red;
                }
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
                var success = await _connection.Register(email, username, firstName, lastName, password);
                RegisterResult.Text = success ? "Zarejestrowano!" : "Użytkownik już istnieje.";
                RegisterResult.Foreground = success ? Brushes.Green : Brushes.Red;
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
