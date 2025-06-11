using Bookstore.Models;
using Bookstore.Services;
using Bookstore.SignalRHub;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Bookstore.ViewModels
{
    public class CartViewModel : INotifyPropertyChanged
    {
        private readonly ConnectionHub _connectionHub;
        private readonly ApiService _apiService;
        private readonly BookService _bookService;
        private readonly OrderService _orderService;
        private CartService _cartService;
        private Guid _userId;

        public ObservableCollection<OfferItemViewModel> CartOffers { get; }
            = new ObservableCollection<OfferItemViewModel>();

        public ICommand CreateOrderCommand { get; private set; }
        public ICommand RemoveFromCartCommand { get; private set; }

        public CartViewModel(ConnectionHub connectionHub)
        {
            _connectionHub = connectionHub;
            _apiService = new ApiService();
            _bookService = new BookService();
            _orderService = new OrderService();

            CreateOrderCommand = new RelayCommand(async _ => await CreateOrderAsync(), _ => CartOffers.Any());
            RemoveFromCartCommand = new RelayCommand(async param => await RemoveFromCartAsync(param as OfferItemViewModel));

            InitializeAsync();
        }

        private async Task InitializeAsync()
        {
            try
            {
                _userId = await _connectionHub.GetUserId();
                _cartService = new CartService(_userId);
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
                await vm.LoadBookAsync();
                await vm.LoadImageAsync();
                CartOffers.Add(vm);
            }

            OnPropertyChanged(nameof(CartOffers));
            ((RelayCommand)CreateOrderCommand).RaiseCanExecuteChanged();
        }

        private async Task CreateOrderAsync()
        {
            if (!CartOffers.Any())
            {
                MessageBox.Show("Koszyk jest pusty!", "Informacja", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var result = MessageBox.Show("Czy chcesz utworzyć zamówienie z produktów w koszyku?",
                "Potwierdzenie", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    var offerIds = CartOffers.Select(co => co.Model.Id).ToList();
                    var success = await _orderService.CreateOrderFromCartAsync(_userId, offerIds);

                    if (success)
                    {
                        MessageBox.Show("Zamówienie zostało utworzone pomyślnie!", "Sukces",
                            MessageBoxButton.OK, MessageBoxImage.Information);

                        // Clear cart after successful order
                        _cartService.SaveCart(new List<Offer>());
                        await RefreshAsync();
                    }
                    else
                    {
                        MessageBox.Show("Nie udało się utworzyć zamówienia.", "Błąd",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Błąd podczas tworzenia zamówienia: {ex.Message}", "Błąd",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private async Task RemoveFromCartAsync(OfferItemViewModel offerViewModel)
        {
            if (offerViewModel != null)
            {
                _cartService.Remove(offerViewModel.Model);
                await RefreshAsync();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
