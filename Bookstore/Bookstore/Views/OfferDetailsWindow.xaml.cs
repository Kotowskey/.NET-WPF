using Bookstore.ViewModels;
using System.Windows;

namespace Bookstore.Views
{
    public partial class OfferDetailsWindow : Window
    {
        public OfferDetailsWindow(OfferItemViewModel offerViewModel, MyOfferViewModel parentViewModel)
        {
            InitializeComponent();
            DataContext = new OfferDetailsViewModel(offerViewModel, parentViewModel);
        }
    }
}