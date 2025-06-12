using Bookstore.Models;
using Bookstore.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Bookstore.ViewModels
{
    public class EditOrderViewModel : INotifyPropertyChanged
    {
        private readonly OrderService _orderService;
        private readonly Order _originalOrder;
        private readonly Action _refreshOrdersCallback;

        private string _selectedStatus;
        public string SelectedStatus
        {
            get => _selectedStatus;
            set
            {
                _selectedStatus = value;
                OnPropertyChanged(nameof(SelectedStatus));
            }
        }

        public List<string> AvailableStatuses { get; private set; }

        public int OrderId { get; private set; }
        public string CustomerName { get; private set; }
        public string OrderItems { get; private set; }
        public string OrderDate { get; private set; }
        public string OrderPrice { get; private set; }

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        public EditOrderViewModel(Order order, Action refreshOrdersCallback)
        {
            _orderService = new OrderService();
            _originalOrder = order;
            _refreshOrdersCallback = refreshOrdersCallback;

            // Inicjalizacja właściwości
            OrderId = order.Id;
            CustomerName = order.CustomerName;
            OrderItems = order.BookTitle;
            OrderDate = order.OrderDateDisplay;
            OrderPrice = order.PriceDisplay;
            SelectedStatus = order.Status;

            // Pobierz dostępne statusy
            AvailableStatuses = _orderService.GetAvailableOrderStatuses();

            // Inicjalizacja komend
            SaveCommand = new RelayCommand(async _ => await SaveChangesAsync());
            CancelCommand = new RelayCommand(_ => CloseWindow());
        }

        private async Task SaveChangesAsync()
        {
            try
            {
                // Sprawdź, czy status się zmienił
                if (SelectedStatus == _originalOrder.Status)
                {
                    MessageBox.Show("Nie wprowadzono zmian w statusie zamówienia.", "Informacja", 
                        MessageBoxButton.OK, MessageBoxImage.Information);
                    CloseWindow();
                    return;
                }

                // Pokaż komunikat potwierdzający
                var result = MessageBox.Show($"Czy na pewno chcesz zmienić status zamówienia #{OrderId} z '{_originalOrder.Status}' na '{SelectedStatus}'?", 
                    "Potwierdź zmianę", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    // Zapisz zmiany w bazie danych
                    bool success = await _orderService.UpdateOrderStatusAsync(OrderId, SelectedStatus);

                    if (success)
                    {
                        MessageBox.Show("Status zamówienia został zaktualizowany pomyślnie.", "Sukces", 
                            MessageBoxButton.OK, MessageBoxImage.Information);
                        
                        // Wywołaj callback do odświeżenia listy zamówień
                        _refreshOrdersCallback?.Invoke();
                        
                        CloseWindow();
                    }
                    else
                    {
                        MessageBox.Show("Nie udało się zaktualizować statusu zamówienia.", "Błąd", 
                            MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Wystąpił błąd podczas aktualizacji zamówienia: {ex.Message}", "Błąd", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CloseWindow()
        {
            foreach (Window window in Application.Current.Windows)
            {
                if (window.DataContext == this)
                {
                    window.Close();
                    break;
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
