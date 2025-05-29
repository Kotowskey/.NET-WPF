using Bookstore.Models;
using Bookstore.ViewModels;
using System.Windows;
using Bookstore.Models;

namespace Bookstore.Views
{
    public partial class EditUserWindow : Window
    {
        public EditUserWindow(UserModel user, UserDetailsViewModel userDetailsViewModel)
        {
            InitializeComponent();
            DataContext = new EditUserViewModel(user, userDetailsViewModel);
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }
    }
}
