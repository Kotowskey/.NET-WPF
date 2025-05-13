using Bookstore.SignalRHub;
using Bookstore.Translation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Bookstore
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ConnectionHub _connection;
        private bool _isAdmin;

        public MainWindow(bool IsAdmin)
        {
            InitializeComponent();
            _isAdmin = IsAdmin;
            this.Loaded += MainWindow_Loaded;
            CheckAdmin();
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
        private void CheckAdmin()
        {
            // Sprawdzenie, czy użytkownik jest administratorem  
            if (_isAdmin)
            {
                // Umożliwienie dostępu do widoku statystyk  
                if (AdminItem != null)
                {
                    AdminItem.Visibility = Visibility.Visible;
                }
            }
            else
            {
                // Ukrycie widoku statystyk  
                if (AdminItem != null)
                {
                    AdminItem.Visibility = Visibility.Collapsed;
                }
            }
        }

        private void NavListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DashboardItem != null && DashboardItem.IsSelected && DashboardView != null)
            {
                HideAllViews();
                DashboardView.Visibility = Visibility.Visible;
            }
            else if (BooksItem != null && BooksItem.IsSelected && BooksView != null)
            {
                HideAllViews();
                BooksView.Visibility = Visibility.Visible;
            }
            else if (OffersItem != null && OffersItem.IsSelected && OffersView != null)
            {
                HideAllViews();
                OffersView.Visibility = Visibility.Visible;
            }
            else if (CustomersItem != null && CustomersItem.IsSelected && CustomersView != null)
            {
                HideAllViews();
                CustomersView.Visibility = Visibility.Visible;
            }
            else if (OrdersItem != null && OrdersItem.IsSelected && OrdersView != null)
            {
                HideAllViews();
                OrdersView.Visibility = Visibility.Visible;
            }
            else if (StatsItem != null && StatsItem.IsSelected && StatsView != null)
            {
                HideAllViews();
                StatsView.Visibility = Visibility.Visible;
            }
            else if (AdminItem != null && AdminItem.IsSelected && AdminView != null)
            {
                HideAllViews();
                AdminView.Visibility = Visibility.Visible;
            }
        }

        private void HideAllViews()
        {
            if (DashboardView != null) DashboardView.Visibility = Visibility.Collapsed;
            if (BooksView != null) BooksView.Visibility = Visibility.Collapsed;
            if (OffersView != null) OffersView.Visibility = Visibility.Collapsed;
            if (CustomersView != null) CustomersView.Visibility = Visibility.Collapsed;
            if (OrdersView != null) OrdersView.Visibility = Visibility.Collapsed;
            if (StatsView != null) StatsView.Visibility = Visibility.Collapsed;
            if (AdminView != null) AdminView.Visibility = Visibility.Collapsed;
        }

        private void GoToOffers_Click(object sender, RoutedEventArgs e)
        {
            if (NavListView != null && OffersItem != null)
            {
                NavListView.SelectedItem = OffersItem;
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