using Bookstore.ViewModels;
using System.Windows.Controls;

namespace Bookstore.Views
{
    /// <summary>
    /// Logika interakcji dla klasy OrdersPage.xaml
    /// </summary>
    public partial class OrdersPage : UserControl
    {
        public OrdersViewModel ViewModel { get; set; }

        public OrdersPage()
        {
            InitializeComponent();
            ViewModel = new OrdersViewModel();
            DataContext = ViewModel;
        }

        private async void UserControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            await ViewModel.LoadOrdersAsync();
        }
    }
}
