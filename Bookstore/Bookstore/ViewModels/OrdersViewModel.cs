using Bookstore.Models;
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
            Orders = new ObservableCollection<Order>();
            _allOrders = new ObservableCollection<Order>();

            LoadOrdersCommand = new RelayCommand(async _ => await LoadOrdersAsync());
            RefreshCommand = new RelayCommand(async _ => await LoadOrdersAsync());

            LoadOrdersAsync().ConfigureAwait(false);
        }

        private async Task LoadOrdersAsync()
        {
            IsLoading = true;
            NoResults = false;

            try
            {
                // Symulacja danych - w rzeczywistej aplikacji pobierasz z API
                await Task.Delay(1000); // Symulacja opóźnienia

                var sampleOrders = new[]
                {
                    new Order { Id = 1, CustomerName = "Jan Kowalski", BookTitle = "Wiedźmin", OrderDate = DateTime.Now.AddDays(-5), Price = 39.99m, Status = "Wysłane", CustomerId = Guid.NewGuid() },
                    new Order { Id = 2, CustomerName = "Anna Nowak", BookTitle = "Hobbit", OrderDate = DateTime.Now.AddDays(-3), Price = 29.99m, Status = "W przygotowaniu", CustomerId = Guid.NewGuid() },
                    new Order { Id = 3, CustomerName = "Piotr Wiśniewski", BookTitle = "1984", OrderDate = DateTime.Now.AddDays(-1), Price = 24.99m, Status = "Oczekuje", CustomerId = Guid.NewGuid() },
                    new Order { Id = 4, CustomerName = "Maria Wójcik", BookTitle = "Lalka", OrderDate = DateTime.Now, Price = 19.99m, Status = "Nowe", CustomerId = Guid.NewGuid() }
                };

                _allOrders.Clear();
                Orders.Clear();

                foreach (var order in sampleOrders)
                {
                    _allOrders.Add(order);
                    Orders.Add(order);
                }

                UpdateResultsVisibility();
            }
            catch (Exception ex)
            {
                // W rzeczywistej aplikacji obsługa błędów
                System.Windows.MessageBox.Show($"Błąd podczas ładowania zamówień: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void FilterOrders()
        {
            if (_allOrders == null) return;

            if (string.IsNullOrWhiteSpace(SearchText))
            {
                Orders = new ObservableCollection<Order>(_allOrders);
            }
            else
            {
                var searchText = SearchText.ToLower();
                var filteredOrders = _allOrders.Where(o =>
                    (o.CustomerName?.ToLower().Contains(searchText) ?? false) ||
                    (o.BookTitle?.ToLower().Contains(searchText) ?? false) ||
                    (o.Status?.ToLower().Contains(searchText) ?? false)
                ).ToList();

                Orders = new ObservableCollection<Order>(filteredOrders);
            }

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
