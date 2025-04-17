using Bookstore.Models;
using Bookstore.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Bookstore.Views
{
    /// <summary>
    /// Logika interakcji dla klasy BooksPage.xaml
    /// </summary>
    public partial class BooksPage : UserControl
    {
        private readonly BookService _bookService;
        private List<BookModel> _allBooks;
        public BooksPage()
        {
            InitializeComponent();
            _bookService = new BookService();

            SearchBox.TextChanged += SearchBox_TextChanged;

            // Load books when the page is loaded
            Loaded += async (s, e) => await LoadBooks();
        }
        private async Task LoadBooks()
        {
            LoadingCard.Visibility = Visibility.Visible;
            NoResultsText.Visibility = Visibility.Collapsed;
            BooksListView.Visibility = Visibility.Collapsed;

            try
            {
                _allBooks = await _bookService.GetAllAsync();
                UpdateBooksDisplay(_allBooks);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Nie można załadować książek: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                LoadingCard.Visibility = Visibility.Collapsed;
                BooksListView.Visibility = Visibility.Visible;
            }
        }

        private void UpdateBooksDisplay(List<BookModel> books)
        {
            BooksListView.ItemsSource = books;

            if (books == null || books.Count == 0)
            {
                NoResultsText.Visibility = Visibility.Visible;
                BooksListView.Visibility = Visibility.Collapsed;
            }
            else
            {
                NoResultsText.Visibility = Visibility.Collapsed;
                BooksListView.Visibility = Visibility.Visible;
            }
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_allBooks == null) return;

            var searchText = SearchBox.Text.ToLower();
            if (string.IsNullOrWhiteSpace(searchText))
            {
                UpdateBooksDisplay(_allBooks);
                return;
            }

            var filteredBooks = _allBooks.Where(o =>
                (o.Title?.ToLower().Contains(searchText) ?? false)).ToList();

            UpdateBooksDisplay(filteredBooks);
        }

        private async void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            await LoadBooks();
        }

        private void BooksListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (BooksListView.SelectedItem is BookModel selectedBook)
            {
                MessageBox.Show($"Wybrano książkę: {selectedBook.Title}");
            }
        }
    }
}

