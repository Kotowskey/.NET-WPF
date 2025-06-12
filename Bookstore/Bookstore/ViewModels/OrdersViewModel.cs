using Bookstore.Models;
using Bookstore.Services;
using Bookstore.Views;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Collections.Generic;

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
        private bool _isAdmin;

        public ObservableCollection<Order> Orders
        {
            get => _orders;
            set 
            { 
                _orders = value; 
                OnPropertyChanged(nameof(Orders));
                UpdateResultsVisibility();
            }
        }

        public string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;
                OnPropertyChanged(nameof(SearchText));
                _ = Task.Run(async () => await FilterOrdersAsync());
            }
        }

        public bool IsLoading
        {
            get => _isLoading;
            set 
            { 
                _isLoading = value; 
                OnPropertyChanged(nameof(IsLoading));
            }
        }

        public bool NoResults
        {
            get => _noResults;
            set 
            { 
                _noResults = value; 
                OnPropertyChanged(nameof(NoResults));
            }
        }

        public bool IsAdmin
        {
            get => _isAdmin;
            set
            {
                _isAdmin = value;
                OnPropertyChanged(nameof(IsAdmin));
            }
        }

        public ICommand RefreshCommand { get; private set; }
        public ICommand SearchCommand { get; private set; }
        public ICommand EditOrderCommand { get; private set; }

        public OrdersViewModel()
        {
            _orderService = new OrderService();
            _orders = new ObservableCollection<Order>();
            _allOrders = new ObservableCollection<Order>();

            RefreshCommand = new RelayCommand(async _ => await LoadOrdersAsync());
            SearchCommand = new RelayCommand(async _ => await FilterOrdersAsync());
            EditOrderCommand = new RelayCommand(OpenEditOrderWindow);

            // Load orders on initialization
            _ = Task.Run(async () => 
            {
                // Check if user is admin
                IsAdmin = await App.Auth.IsAdminAsync();
                await LoadOrdersAsync();
            });
        }

        public async Task LoadOrdersAsync()
        {
            try
            {
                IsLoading = true;
                NoResults = false;

                List<Order> orders;

                // Check if user is admin
                bool isAdmin = await App.Auth.IsAdminAsync();
                
                if (isAdmin)
                {
                    // Admin sees all orders
                    orders = await _orderService.GetAllAsync();
                }
                else
                {
                    // Regular user sees only their orders
                    var userId = await App.Auth.GetUserIdAsync();
                    orders = await _orderService.GetByUserIdAsync(userId);
                }

                // Update on UI thread
                Application.Current.Dispatcher.Invoke(() =>
                {
                    _allOrders.Clear();
                    Orders.Clear();

                    foreach (var order in orders)
                    {
                        _allOrders.Add(order);
                        Orders.Add(order);
                    }

                    UpdateResultsVisibility();
                });
            }
            catch (Exception ex)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    MessageBox.Show($"Błąd podczas ładowania zamówień: {ex.Message}",
                        "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                });
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task FilterOrdersAsync()
        {
            if (_allOrders == null) return;

            try
            {
                var filteredOrders = string.IsNullOrWhiteSpace(SearchText)
                    ? _allOrders.ToList()
                    : await _orderService.SearchOrdersAsync(SearchText);

                Application.Current.Dispatcher.Invoke(() =>
                {
                    Orders.Clear();
                    foreach (var order in filteredOrders)
                    {
                        Orders.Add(order);
                    }
                    UpdateResultsVisibility();
                });
            }
            catch (Exception ex)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    MessageBox.Show($"Błąd podczas wyszukiwania zamówień: {ex.Message}",
                        "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                });
            }
        }

        private void UpdateResultsVisibility()
        {
            NoResults = Orders == null || Orders.Count == 0;
        }

        private void OpenEditOrderWindow(object parameter)
        {
            if (parameter is Order order)
            {
                var editWindow = new EditOrderWindow(order, async () => await LoadOrdersAsync());
                editWindow.Owner = Application.Current.MainWindow;
                editWindow.ShowDialog();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
