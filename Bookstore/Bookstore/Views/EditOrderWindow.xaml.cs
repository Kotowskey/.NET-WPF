using Bookstore.Models;
using Bookstore.ViewModels;
using System;
using System.Windows;

namespace Bookstore.Views
{
    /// <summary>
    /// Interaction logic for EditOrderWindow.xaml
    /// </summary>
    public partial class EditOrderWindow : Window
    {
        public EditOrderWindow(Order order, Action refreshCallback)
        {
            InitializeComponent();
            DataContext = new EditOrderViewModel(order, refreshCallback);
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            // Obsługiwane przez komendę w ViewModel
        }
    }
}
