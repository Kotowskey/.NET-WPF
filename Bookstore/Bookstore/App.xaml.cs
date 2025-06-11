using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Bookstore;
using Bookstore.Translation;
using Bookstore.Services;

namespace Bookstore
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static AuthService Auth { get; private set; }
        public static UserService Users { get; private set; }
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            LocalizationManager.ChangeLanguage("pl");
            Auth = new AuthService();
            Users = new UserService(Auth.CookieContainer);
            var loginWindow = new SingInUp();
            loginWindow.Show();
        }
    }
}
