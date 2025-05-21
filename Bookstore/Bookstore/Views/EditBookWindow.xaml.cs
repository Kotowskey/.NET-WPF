using Bookstore.Models;
using Bookstore.ViewModels;
using System.Windows;

namespace Bookstore.Views
{
    public partial class EditBookWindow : Window
    {
        public EditBookWindow(BookModel book, BookDetailsViewModel bookDetailsViewModel)
        {
            InitializeComponent();
            DataContext = new EditBookViewModel(book, bookDetailsViewModel);
        }
    }
}