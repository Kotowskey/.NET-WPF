using Bookstore.Models;
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
        private StatisticsModel _statistics;

        public StatsPage()
        {
            InitializeComponent();
        }

        // Initialize the connection hub
        public void Initialize()
        {
            LoadData();
        }

        private async void LoadData()
        {
            // Show loading, hide content and error
            LoadingCard.Visibility = Visibility.Visible;
            ContentGrid.Visibility = Visibility.Collapsed;
            ErrorText.Visibility = Visibility.Collapsed;

            try
            {
                _statistics = await App.Auth.GetStatisticsAsync();
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
