using Bookstore.Models;
using Bookstore.Services;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Bookstore.ViewModels
{
    public class OrdersViewModel : INotifyPropertyChanged
    {
        private readonly OrderService _orderService;
        private ObservableCollection<Order> _orders;
        private ObservableCollection<Order> _allOrders;
        private string _searchText;
        private bool _isLoading;
        private bool _noResults;

        public ObservableCollection<Order> Orders
        {
            get => _orders;
            set { _orders = value; OnPropertyChanged(nameof(Orders)); }
        }

        public string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;
                OnPropertyChanged(nameof(SearchText));
                FilterOrders();
            }
        }

        public bool IsLoading
        {
            get => _isLoading;
            set { _isLoading = value; OnPropertyChanged(nameof(IsLoading)); }
        }

        public bool NoResults
        {
            get => _noResults;
            set { _noResults = value; OnPropertyChanged(nameof(NoResults)); }
        }

        public ICommand LoadOrdersCommand { get; private set; }
        public ICommand RefreshCommand { get; private set; }

        public OrdersViewModel()
        {
            _orderService = new OrderService();
            _orders = new ObservableCollection<Order>();
            _allOrders = new ObservableCollection<Order>();

            LoadOrdersCommand = new RelayCommand(async _ => await LoadOrdersAsync());
            RefreshCommand = new RelayCommand(async _ => await LoadOrdersAsync());

            _ = LoadOrdersAsync();
        }

        private async Task LoadOrdersAsync()
        {
            IsLoading = true;
            NoResults = false;

            try
            {
                var orders = await _orderService.GetAllAsync();

                _allOrders.Clear();
                Orders.Clear();

                foreach (var order in orders)
                {
                    _allOrders.Add(order);
                    Orders.Add(order);
                }

                UpdateResultsVisibility();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Błąd podczas ładowania zamówień: {ex.Message}",
                    "Błąd", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void FilterOrders()
        {
            if (_allOrders == null || Orders == null) return;

            Orders.Clear();

            var filtered = string.IsNullOrWhiteSpace(SearchText)
                ? _allOrders
                : _allOrders.Where(o =>
                    (o.CustomerName?.ToLower().Contains(SearchText.ToLower()) ?? false) ||
                    (o.BookTitle?.ToLower().Contains(SearchText.ToLower()) ?? false) ||
                    (o.Status?.ToLower().Contains(SearchText.ToLower()) ?? false)
                );

            foreach (var order in filtered)
                Orders.Add(order);

            UpdateResultsVisibility();
        }

        private void UpdateResultsVisibility()
        {
            NoResults = Orders == null || Orders.Count == 0;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
