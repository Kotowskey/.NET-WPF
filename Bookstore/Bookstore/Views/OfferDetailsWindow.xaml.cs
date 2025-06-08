using Bookstore.Models;
using Bookstore.Services;
using Bookstore.ViewModels;
using System.Windows;

namespace Bookstore.Views
{
    public partial class OfferDetailsWindow : Window
    {
        private readonly ApiService _apiService;
        private readonly BookService _bookService;
        private readonly Offer _offer;
        public OfferDetailsWindow(OfferItemViewModel offerViewModel, MyOfferViewModel parentViewModel)
        {
            InitializeComponent();
            _apiService = new ApiService();
            _bookService = new BookService();
            _offer = offerViewModel.Model;
            DataContext = new OfferDetailsViewModel(offerViewModel, parentViewModel);
        }

        private void EditOffer_Click(object sender, RoutedEventArgs e)
        {
            var editWindow = new AddOfferWindow(_apiService, _bookService, _offer.RequesterId, _offer);
            editWindow.ShowDialog();
        }
    }
}