using Bookstore.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace Bookstore.Views
{
    /// <summary>
    /// Logika interakcji dla klasy OrdersPage.xaml
    /// </summary>
    public partial class OrdersPage : UserControl
    {
        private OrdersViewModel _viewModel;

        public OrdersPage()
        {
            InitializeComponent();
            _viewModel = new OrdersViewModel();
            DataContext = _viewModel;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            // Odświeżenie danych przy załadowaniu kontrolki
            if (_viewModel != null)
            {
                _viewModel.RefreshCommand.Execute(null);
            }
        }
    }
}
