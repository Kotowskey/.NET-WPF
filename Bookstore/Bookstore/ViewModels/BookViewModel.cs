using Bookstore.Models;
using Bookstore.Services;
using Bookstore.Translation;
using Bookstore.Views;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Bookstore.ViewModels
{
    public class BookViewModel : INotifyPropertyChanged
    {
        private readonly BookService _bookService;
        private ObservableCollection<BookModel> _books;
        private ObservableCollection<BookModel> _allBooks;
        private string _searchText;
        private bool _isLoading;
        private bool _noResults;

        public ObservableCollection<BookModel> Books
        {
            get => _books;
            set
            {
                _books = value;
                OnPropertyChanged(nameof(Books));
            }
        }

        public string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;
                OnPropertyChanged(nameof(SearchText));
                FilterBooks();
            }
        }

        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                _isLoading = value;
                OnPropertyChanged(nameof(IsLoading));
            }
        }

        public bool NoResults
        {
            get => _noResults;
            set
            {
                _noResults = value;
                OnPropertyChanged(nameof(NoResults));
            }
        }

        public ICommand LoadBooksCommand { get; private set; }
        public ICommand AddBookCommand { get; private set; }
        public ICommand BookSelectedCommand { get; private set; }

        public BookViewModel()
        {
            _bookService = new BookService();
            Books = new ObservableCollection<BookModel>();

            LoadBooksCommand = new RelayCommand(async param => await LoadBooksAsync());
            AddBookCommand = new RelayCommand(param => OpenAddBookWindow());
            BookSelectedCommand = new RelayCommand(param => OpenBookDetailsWindow(param as BookModel));

            Task.Run(async () => await LoadBooksAsync());
        }

        public async Task LoadBooksAsync()
        {
            IsLoading = true;
            NoResults = false;

            try
            {
                var books = await _bookService.GetAllAsync();
                _allBooks = new ObservableCollection<BookModel>(books);
                Books = _allBooks;
                UpdateResultsVisibility();
            }
            catch (Exception)
            {
                MessageBox.Show(
                    LocalizationManager.Get("BooksLoadingError"),
                    LocalizationManager.Get("Error"),
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void FilterBooks()
        {
            if (_allBooks == null) return;

            if (string.IsNullOrWhiteSpace(SearchText))
            {
                Books = new ObservableCollection<BookModel>(_allBooks);
            }
            else
            {
                var searchText = SearchText.ToLower();
                var filteredBooks = _allBooks.Where(o =>
                    (o.Title?.ToLower().Contains(searchText) ?? false) ||
                    (o.AuthorDisplay?.ToLower().Contains(searchText) ?? false) ||
                    (o.Isbn?.ToLower().Contains(searchText) ?? false)
                ).ToList();

                Books = new ObservableCollection<BookModel>(filteredBooks);
            }

            UpdateResultsVisibility();
        }

        private void UpdateResultsVisibility()
        {
            NoResults = Books == null || Books.Count == 0;
        }

        public void RemoveBookFromList(int bookId)
        {
            var bookToRemove = _allBooks.FirstOrDefault(b => b.Id == bookId);
            if (bookToRemove != null)
            {
                _allBooks.Remove(bookToRemove);
                FilterBooks();
            }
        }

        private void OpenAddBookWindow()
        {
            var addBookWindow = new AddBookWindow();
            addBookWindow.ShowDialog();


            Task.Run(async () => await LoadBooksAsync());
        }

        private void OpenBookDetailsWindow(BookModel selectedBook)
        {
            if (selectedBook != null)
            {
                var bookDetailsWindow = new BookDetailsWindow(selectedBook, this);
                bookDetailsWindow.ShowDialog();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}