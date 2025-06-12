using Bookstore.Models;
using Bookstore.Services;
using Bookstore.Translation;
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
        private readonly ApiService _apiService;
        private readonly BookService _bookService;
        private CartService _cartService;
        private Guid _userId;

        public ObservableCollection<OfferItemViewModel> CartOffers { get; }
            = new ObservableCollection<OfferItemViewModel>();

        public ICommand AddOfferCommand { get; }
        public ICommand RemoveFromCartCommand { get; } // Dodane

        public CartViewModel()
        {
            _apiService = new ApiService();
            _bookService = new BookService();
            AddOfferCommand = new RelayCommand(async _ => await CreateOrderFromCart());
            RemoveFromCartCommand = new RelayCommand(RemoveFromCart); // Dodane
            InitializeAsync();
        }

        private async Task InitializeAsync()
        {
            try
            {
                _userId = await App.Auth.GetUserIdAsync();
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

                // ładujemy książkę i obrazek
                await vm.LoadBookAsync();
                await vm.LoadImageAsync();

                CartOffers.Add(vm);
            }

            OnPropertyChanged(nameof(CartOffers));
        }

        private async Task CreateOrderFromCart()
        {
            try
            {
                if (!CartOffers.Any())
                {
                    MessageBox.Show(LocalizationManager.Get("CartEmpty"), LocalizationManager.Get("Information"), MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                var result = MessageBox.Show("Czy chcesz utworzyć zamówienie z przedmiotów w koszyku?",
                    "Potwierdzenie", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    var createOrderDto = new CreateOrderDto
                    {
                        BuyerId = _userId,
                        OfferIds = CartOffers.Select(co => co.Model.Id).ToList()
                    };

                    var success = await _apiService.CreateOrderFromCartAsync(createOrderDto);
                    if (success)
                    {
                        MessageBox.Show("Zamówienie zostało utworzone pomyślnie!", "Sukces", MessageBoxButton.OK, MessageBoxImage.Information);

                        // Clear cart after successful order creation
                        _cartService.SaveCart(new List<Offer>());
                        await RefreshAsync();
                    }
                    else
                    {
                        MessageBox.Show("Nie udało się utworzyć zamówienia.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd podczas tworzenia zamówienia: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void RemoveFromCart(object parameter)
        {
            if (parameter is OfferItemViewModel vm)
            {
                var result = MessageBox.Show($"Czy na pewno chcesz usunąć '{vm.Name}' z koszyka?", "Potwierdź", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    _cartService.Remove(vm.Model);
                    CartOffers.Remove(vm);
                    OnPropertyChanged(nameof(CartOffers));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    public class CreateOrderDto
    {
        public Guid BuyerId { get; set; }
        public List<int> OfferIds { get; set; } = new List<int>();
    }
}
