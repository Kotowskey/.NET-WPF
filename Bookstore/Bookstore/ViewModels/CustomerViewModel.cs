using Bookstore.Models;
using Bookstore.Services;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;

namespace Bookstore.ViewModels
{
    public class CustomerViewModel : INotifyPropertyChanged
    {
        private readonly UserService _userService;
        private ObservableCollection<UserModel> _customers;
        private bool _isLoading;
        private bool _noResults;

        public ObservableCollection<UserModel> Customers
        {
            get => _customers;
            set
            {
                _customers = value;
                OnPropertyChanged(nameof(Customers));
                NoResults = _customers == null || _customers.Count == 0;
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

        public CustomerViewModel()
        {
            _userService = new UserService(App.Auth.CookieContainer);
            Customers = new ObservableCollection<UserModel>();
        }

        public async Task LoadCustomersAsync()
        {
            try
            {
                IsLoading = true;
                NoResults = false;

                var customers = await _userService.GetAllAsync();
                Customers = new ObservableCollection<UserModel>(customers);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd podczas ładowania klientów: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        public async Task AddCustomerAsync(UserModel customer)
        {
            try
            {
                IsLoading = true;
                var addedCustomer = await _userService.AddUserAsync(customer);

                if (addedCustomer != null)
                {
                    await LoadCustomersAsync(); // Odświeżamy listę
                    MessageBox.Show("Klient został dodany.", "Sukces", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Nie udało się dodać klienta.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd podczas dodawania klienta: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        public async Task EditCustomerAsync(UserModel customer)
        {
            try
            {
                IsLoading = true;
                var updatedCustomer = await _userService.EditUserAsync(customer.Id, customer);

                if (updatedCustomer != null)
                {
                    await LoadCustomersAsync(); // Odświeżamy listę
                    MessageBox.Show("Klient został zaktualizowany.", "Sukces", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Nie udało się zaktualizować klienta.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd podczas aktualizacji klienta: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        public async Task DeleteCustomerAsync(UserModel customer)
        {
            try
            {
                IsLoading = true;
                var success = await _userService.DeleteUserAsync(customer.Id);

                if (success)
                {
                    await LoadCustomersAsync(); // Odświeżamy listę
                    MessageBox.Show("Klient został usunięty.", "Sukces", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Nie udało się usunąć klienta.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd podczas usuwania klienta: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        public async Task SearchCustomersAsync(string searchPhrase)
        {
            try
            {
                IsLoading = true;
                NoResults = false;

                var customers = await _userService.SearchUsersAsync(searchPhrase);
                Customers = new ObservableCollection<UserModel>(customers);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd podczas wyszukiwania klientów: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
