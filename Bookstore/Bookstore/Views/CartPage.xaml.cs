using Bookstore.ViewModels;
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

namespace Bookstore.Views
{
    /// <summary>
    /// Logika interakcji dla klasy CartPage.xaml
    /// </summary>
    public partial class CartPage : UserControl
    {
        private CartViewModel Vm => (CartViewModel)DataContext;
        public CartPage()
        {
            InitializeComponent();
            DataContext = new CartViewModel();
            this.IsVisibleChanged += CartPage_IsVisibleChanged;
        }
        private async void CartPage_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue == true)
            {
                await Vm.RefreshAsync();
            }
        }

    }
}
