using Bookstore.Models;
using Bookstore.SignalRHub;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Bookstore.Views
{
    /// <summary>
    /// Interaction logic for StatsPage.xaml
    /// </summary>
    public partial class StatsPage : UserControl
    {
        private ConnectionHub _connection;
        private StatisticsModel _statistics;

        public StatsPage()
        {
            InitializeComponent();
        }

        // Initialize the connection hub
        public void Initialize(ConnectionHub connectionHub)
        {
            _connection = connectionHub;
            LoadData();
        }

        private async void LoadData()
        {
            if (_connection == null)
                return;

            // Show loading, hide content and error
            LoadingCard.Visibility = Visibility.Visible;
            ContentGrid.Visibility = Visibility.Collapsed;
            ErrorText.Visibility = Visibility.Collapsed;

            try
            {
                _statistics = await _connection.GetStatistics();
                DataContext = _statistics;
                
                // Show content, hide loading and error
                ContentGrid.Visibility = Visibility.Visible;
                LoadingCard.Visibility = Visibility.Collapsed;
            }
            catch (Exception ex)
            {
                // Show error, hide loading and content
                ErrorText.Text = $"Błąd: {ex.Message}";
                ErrorText.Visibility = Visibility.Visible;
                LoadingCard.Visibility = Visibility.Collapsed;
                ContentGrid.Visibility = Visibility.Collapsed;
            }
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            LoadData();
        }
    }
}
