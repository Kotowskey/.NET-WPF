using Bookstore.Models;
using Bookstore.ViewModels;
using System.Windows;
using Bookstore.Models;

namespace Bookstore.Views
{
    public partial class UserDetailsWindow : Window
    {
        public UserDetailsWindow(UserModel user, UserViewModel parentViewModel)
        {
            InitializeComponent();
            DataContext = new UserDetailsViewModel(user, parentViewModel);
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
