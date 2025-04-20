using Bookstore.Models;
using Bookstore.ViewModels;
using System.Windows;

namespace Bookstore.Views
{
    /// <summary>
    /// Interaction logic for BookDetailsWindow.xaml
    /// </summary>
    public partial class BookDetailsWindow : Window
    {
        public BookDetailsWindow(BookModel book, BookViewModel parentViewModel)
        {
            InitializeComponent();
            DataContext = new BookDetailsViewModel(book, parentViewModel);
        }
    }
}