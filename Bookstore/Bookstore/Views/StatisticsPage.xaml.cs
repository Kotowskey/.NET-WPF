using Bookstore.Services;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Bookstore.Views
{
    /// <summary>
    /// Interaction logic for StatisticsPage.xaml
    /// </summary>
    public partial class StatisticsPage : UserControl
    {
        private readonly StatisticsService _statisticsService;

        public StatisticsPage()
        {
            InitializeComponent();
            // Initialize the service with the hub URL
            _statisticsService = new StatisticsService("http://localhost:5000/statisticsHub");
            Loaded += StatisticsPage_Loaded;
        }

        private async void StatisticsPage_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadStatisticsAsync();
        }

        private async Task LoadStatisticsAsync()
        {
            try
            {
                // Show loading indicators
                BooksCountText.Text = "Ładowanie...";
                OffersCountText.Text = "Ładowanie...";
                UsersCountText.Text = "Ładowanie...";

                // Get statistics from backend
                var statistics = await _statisticsService.GetStatisticsAsync();

                // Update the UI
                BooksCountText.Text = statistics.BooksCount.ToString();
                OffersCountText.Text = statistics.OffersCount.ToString();
                UsersCountText.Text = statistics.UsersCount.ToString();
            }
            catch (Exception ex)
            {
                // Handle errors
                MessageBox.Show($"Błąd podczas pobierania statystyk: {ex.Message}", "Błąd",
                    MessageBoxButton.OK, MessageBoxImage.Error);

                // Set fallback values
                BooksCountText.Text = "N/A";
                OffersCountText.Text = "N/A";
                UsersCountText.Text = "N/A";
            }
        }

        // Add a refresh button handler
        private async void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            await LoadStatisticsAsync();
        }
    }
}
