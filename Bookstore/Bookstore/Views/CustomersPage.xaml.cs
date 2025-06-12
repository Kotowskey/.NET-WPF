using Bookstore.Models;
using Bookstore.ViewModels;
using Bookstore.Translation;
using System;
using System.Windows;
using System.Windows.Controls;

namespace Bookstore.Views
{
    /// <summary>
    /// Logika interakcji dla klasy CustomersPage.xaml
    /// </summary>
    public partial class CustomersPage : UserControl
    {
        public CustomerViewModel ViewModel { get; set; }

        public CustomersPage()
        {
            InitializeComponent();
            ViewModel = new CustomerViewModel();
            DataContext = ViewModel;

            Loaded += CustomersPage_Loaded;
        }

        private async void CustomersPage_Loaded(object sender, RoutedEventArgs e)
        {
            await ViewModel.LoadCustomersAsync();
        }

        private async void AddCustomer_Click(object sender, RoutedEventArgs e)
        {
            var newCustomer = new UserModel
            {
                Email = "",
                Username = "",
                FirstName = "",
                LastName = "",
                Password = "",
                IsAdmin = false,
                StateEnum = Bookstore.Models.Enums.StateEnum.Active
            };
            var editWindow = new EditUserWindow(newCustomer, null);
            if (editWindow.ShowDialog() == true)
            {
                await ViewModel.AddCustomerAsync(newCustomer);
            }
        }

        private async void EditCustomer_Click(object sender, RoutedEventArgs e)
        {
            if (CustomersDataGrid.SelectedItem is UserModel selectedCustomer)
            {
                var customerCopy = new UserModel
                {
                    Id = selectedCustomer.Id,
                    Email = selectedCustomer.Email,
                    Username = selectedCustomer.Username,
                    FirstName = selectedCustomer.FirstName,
                    LastName = selectedCustomer.LastName,
                    Password = selectedCustomer.Password,
                    IsAdmin = selectedCustomer.IsAdmin,
                    StateEnum = selectedCustomer.StateEnum
                };
                var editWindow = new EditUserWindow(customerCopy, null);
                if (editWindow.ShowDialog() == true)
                {
                    await ViewModel.EditCustomerAsync(customerCopy);
                }
            }
            else
            {
                MessageBox.Show(LocalizationManager.Get("SelectCustomer"));
            }
        }

        private async void DeleteCustomer_Click(object sender, RoutedEventArgs e)
        {
            if (CustomersDataGrid.SelectedItem is UserModel selectedCustomer)
            {
                var result = MessageBox.Show(LocalizationManager.Get("ConfirmDeleteCustomer"), LocalizationManager.Get("Confirm"), MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    await ViewModel.DeleteCustomerAsync(selectedCustomer);
                }
            }
            else
            {
                MessageBox.Show(LocalizationManager.Get("SelectCustomerDelete"));
            }
        }

        private void ShowDetails_Click(object sender, RoutedEventArgs e)
        {
            if (CustomersDataGrid.SelectedItem is UserModel selectedCustomer)
            {
                var detailsWindow = new UserDetailsWindow(selectedCustomer, null);
                detailsWindow.ShowDialog();
            }
            else
            {
                MessageBox.Show(LocalizationManager.Get("SelectCustomerDetails"));
            }
        }

        private async void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var text = (sender as TextBox)?.Text ?? "";
            if (text != "Szukaj...")
            {
                await ViewModel.SearchCustomersAsync(text);
            }
        }

        private void SearchBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (SearchBox.Text == "Szukaj...")
            {
                SearchBox.Text = "";
                SearchBox.Foreground = System.Windows.Media.Brushes.Black;
            }
        }

        private void SearchBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(SearchBox.Text))
            {
                SearchBox.Text = "Szukaj...";
                SearchBox.Foreground = System.Windows.Media.Brushes.Gray;
            }
        }
    }
}
