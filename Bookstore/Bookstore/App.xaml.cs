using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Bookstore.SignalRHub;
using Bookstore;

namespace Bookstore
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private ConnectionHub _connectionHub;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            _connectionHub = new ConnectionHub();
            var loginWindow = new SingInUp(_connectionHub);
            loginWindow.Show();
        }
    }
}
