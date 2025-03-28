using System.Windows;
using System.Windows.Controls;

namespace Bookstore
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // Inicjalizacja początkowego widoku
            HideAllViews();
            if (DashboardView != null)
            {
                DashboardView.Visibility = Visibility.Visible;
            }
        }

        private void NavListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            HideAllViews();

            // Sprawdzamy każdy element przed zmianą jego widoczności
            if (DashboardItem != null && DashboardItem.IsSelected && DashboardView != null)
            {
                DashboardView.Visibility = Visibility.Visible;
            }
            else if (BooksItem != null && BooksItem.IsSelected && BooksView != null)
            {
                BooksView.Visibility = Visibility.Visible;
            }
            else if (OffersItem != null && OffersItem.IsSelected && OffersView != null)
            {
                OffersView.Visibility = Visibility.Visible;
            }
            else if (CustomersItem != null && CustomersItem.IsSelected && CustomersView != null)
            {
                CustomersView.Visibility = Visibility.Visible;
            }
            else if (OrdersItem != null && OrdersItem.IsSelected && OrdersView != null)
            {
                OrdersView.Visibility = Visibility.Visible;
            }
            else if (StatsItem != null && StatsItem.IsSelected && StatsView != null)
            {
                StatsView.Visibility = Visibility.Visible;
            }
        }

        private void HideAllViews()
        {
            // Sprawdzamy każdy element przed zmianą jego widoczności
            if (DashboardView != null) DashboardView.Visibility = Visibility.Collapsed;
            if (BooksView != null) BooksView.Visibility = Visibility.Collapsed;
            if (OffersView != null) OffersView.Visibility = Visibility.Collapsed;
            if (CustomersView != null) CustomersView.Visibility = Visibility.Collapsed;
            if (OrdersView != null) OrdersView.Visibility = Visibility.Collapsed;
            if (StatsView != null) StatsView.Visibility = Visibility.Collapsed;
        }

        private void GoToOffers_Click(object sender, RoutedEventArgs e)
        {
            if (NavListView != null && OffersItem != null)
            {
                NavListView.SelectedItem = OffersItem;
            }
        }
    }
}