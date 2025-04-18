using Bookstore.Models;
using Bookstore.Services;
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
using System.Windows.Shapes;

namespace Bookstore.Views
{
    /// <summary>
    /// Logika interakcji dla klasy BookDetailsWindow.xaml
    /// </summary>
    public partial class BookDetailsWindow : Window
    {
        private readonly BookService _bookService;
        private readonly BookModel _selectedBook;
        private readonly BooksPage _booksPage;
        public BookDetailsWindow(BookModel selectedBook, BooksPage booksPage)
        {
            InitializeComponent();
            _bookService = new BookService();
            _selectedBook = selectedBook;
            _booksPage = booksPage;

            BookTitle.Text = selectedBook.Title;
            BookAuthor.Text = selectedBook.AuthorDisplay;
            BookDescription.Text = selectedBook.Description;
            BookPublicationYear.Text = selectedBook.PublicationYear;
            BookPublisher.Text = selectedBook.PublisherDisplay;
            BookGenre.Text = selectedBook.GenreDisplay;
            BookSeries.Text = selectedBook?.SeriesDisplay;
            BookIsbn.Text = selectedBook.Isbn;
        }

        private async Task DeleteBook()
        {
            if (_selectedBook != null)
            {
                Console.WriteLine($"Usuwam książkę o ID: {_selectedBook.Id}");
                await _bookService.DeletedAsync(_selectedBook.Id);
            }
        }
        private async void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            await DeleteBook();
            this.Close();
            _booksPage?.RemoveBookFromList(_selectedBook.Id);
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
