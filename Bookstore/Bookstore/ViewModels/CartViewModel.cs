using Bookstore.Models;
using Bookstore.Services;
using Bookstore.SignalRHub;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;

namespace Bookstore.ViewModels
{
    public class CartViewModel : INotifyPropertyChanged
    {
        private readonly ConnectionHub _connectionHub;
        private readonly ApiService _apiService;
        private readonly BookService _bookService;
        private CartService _cartService;

        public ObservableCollection<OfferItemViewModel> CartOffers { get; }
            = new ObservableCollection<OfferItemViewModel>();

        public CartViewModel(ConnectionHub connectionHub)
        {
            _connectionHub = connectionHub;
            _apiService = new ApiService();
            _bookService = new BookService();
            InitializeAsync();
        }

        private async Task InitializeAsync()
        {
            try
            {
                var userId = await _connectionHub.GetUserId();
                _cartService = new CartService(userId);
                await RefreshAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Nie udało się załadować koszyka: {ex.Message}");
            }
        }

        public async Task RefreshAsync()
        {
            var offers = _cartService.LoadCart();
            CartOffers.Clear();

            foreach (var offer in offers)
            {
                var vm = new OfferItemViewModel(offer, _apiService, _bookService);

                // ładujemy książkę i obrazek
                await vm.LoadBookAsync();
                await vm.LoadImageAsync();

                CartOffers.Add(vm);
            }

            OnPropertyChanged(nameof(CartOffers));
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
