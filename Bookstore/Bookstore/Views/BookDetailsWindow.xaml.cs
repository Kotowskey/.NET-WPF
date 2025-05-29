using Bookstore.Models;
using Bookstore.ViewModels;
using System;
using System.Windows;
using Bookstore.SignalRHub;

namespace Bookstore.Views
{
    /// <summary>  
    /// Interaction logic for BookDetailsWindow.xaml  
    /// </summary>  
    public partial class BookDetailsWindow : Window
    {
        private bool _isAdmin;
        public BookDetailsWindow(BookModel book, BookViewModel parentViewModel)
        {
            
            InitializeComponent();
            DataContext = new BookDetailsViewModel(book, parentViewModel);
            IsAdmin();
        }
        private void CheckAdmin()
        {
            // Sprawdzenie, czy użytkownik jest administratorem    
            if (_isAdmin)
            {
                // Umożliwienie dostępu do widoku statystyk    
                EditButton.Visibility = Visibility.Visible;
                DeleteButton.Visibility = Visibility.Visible;
                
            }
            else
            {
                // Ukrycie widoku statystyk    
                EditButton.Visibility = Visibility.Collapsed;
                DeleteButton.Visibility = Visibility.Collapsed;
                
            }
        }
        private async void IsAdmin()
        {
            // Sprawdzenie, czy użytkownik jest administratorem    
            var con = new ConnectionHub();
            _isAdmin = await con.IsAdmin();
            CheckAdmin();

        }
    }
}