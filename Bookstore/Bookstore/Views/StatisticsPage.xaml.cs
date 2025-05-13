using System.Windows.Controls;
using System.Windows;

namespace Bookstore.Views
{
    /// <summary>
    /// Interaction logic for StatisticsPage.xaml
    /// </summary>
    public partial class StatisticsPage : UserControl
    {
        public StatisticsPage()
        {
            InitializeComponent();
            Loaded += StatisticsPage_Loaded;
        }

        private void StatisticsPage_Loaded(object sender, RoutedEventArgs e)
        {
            // Przykładowe dane statystyczne (podmień na pobieranie z serwisu)
            BooksCountText.Text = "123";
            OffersCountText.Text = "45";
            UsersCountText.Text = "67";
        }
    }
}
