using Bookstore.Models;
using Bookstore.ViewModels;
using Bookstore.Translation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Bookstore.Views
{
    /// <summary>
    /// Logika interakcji dla klasy UserPage.xaml
    /// </summary>
    public partial class UserPage : UserControl
    {
        public UserViewModel ViewModel { get; set; }

        public UserPage()
        {
            InitializeComponent();
            ViewModel = new UserViewModel();
            DataContext = ViewModel;

        }

        private async void AddUser_Click(object sender, RoutedEventArgs e)
        {
            var newUser = new UserModel
            {
                Email = "",
                Username = "",
                FirstName = "",
                LastName = "",
                Password = "",
                IsAdmin = false,
                StateEnum = Bookstore.Models.Enums.StateEnum.Active
            };
            var editWindow = new EditUserWindow(newUser, null);
            if (editWindow.ShowDialog() == true)
            {
                await ViewModel.AddUserAsync(newUser);
            }
        }

        private async void EditUser_Click(object sender, RoutedEventArgs e)
        {
            if (UsersDataGrid.SelectedItem is UserModel selectedUser)
            {
                var userCopy = new UserModel
                {
                    Id = selectedUser.Id,
                    Email = selectedUser.Email,
                    Username = selectedUser.Username,
                    FirstName = selectedUser.FirstName,
                    LastName = selectedUser.LastName,
                    Password = selectedUser.Password,
                    IsAdmin = selectedUser.IsAdmin,
                    StateEnum = selectedUser.StateEnum
                };
                var editWindow = new EditUserWindow(userCopy, null);
                if (editWindow.ShowDialog() == true)
                {
                    await ViewModel.EditUserAsync(userCopy);
                }
            }
            else
            {
                MessageBox.Show(LocalizationManager.Get("SelectUser"));
            }
        }

        private async void DeleteUser_Click(object sender, RoutedEventArgs e)
        {
            if (UsersDataGrid.SelectedItem is UserModel selectedUser)
            {
                var result = MessageBox.Show(LocalizationManager.Get("ConfirmDelete"), LocalizationManager.Get("Confirm"), MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    await ViewModel.DeleteUserAsync(selectedUser);
                }
            }
            else
            {
                MessageBox.Show(LocalizationManager.Get("SelectUserDelete"));
            }
        }

        private void ShowDetails_Click(object sender, RoutedEventArgs e)
        {
            if (UsersDataGrid.SelectedItem is UserModel selectedUser)
            {
                var detailsWindow = new UserDetailsWindow(selectedUser, ViewModel);
                detailsWindow.ShowDialog();
            }
            else
            {
                MessageBox.Show(LocalizationManager.Get("SelectUserDetails"));
            }
        }

        private async void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var text = (sender as TextBox)?.Text ?? "";
            await ViewModel.SearchUsersAsync(text);
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
