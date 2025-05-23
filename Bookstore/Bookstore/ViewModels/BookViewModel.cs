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
        private bool _isAdvancedSearchVisible;
        private string _advancedSearchTitle;
        private string _advancedSearchAuthor;
        private string _advancedSearchGenre;
        private string _advancedSearchIsbn;
        private bool _isInitialized = false;

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
                if (_isInitialized && !_isAdvancedSearchVisible)
                {
                    // Wykonaj wyszukiwanie po zmianie tekstu, ale tylko jeśli załadowały się już książki
                    // i nie jest otwarte zaawansowane wyszukiwanie
                    FilterBooks();
                }
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

        public bool IsAdvancedSearchVisible
        {
            get => _isAdvancedSearchVisible;
            set
            {
                _isAdvancedSearchVisible = value;
                OnPropertyChanged(nameof(IsAdvancedSearchVisible));
            }
        }

        public string AdvancedSearchTitle
        {
            get => _advancedSearchTitle;
            set
            {
                _advancedSearchTitle = value;
                OnPropertyChanged(nameof(AdvancedSearchTitle));
            }
        }

        public string AdvancedSearchAuthor
        {
            get => _advancedSearchAuthor;
            set
            {
                _advancedSearchAuthor = value;
                OnPropertyChanged(nameof(AdvancedSearchAuthor));
            }
        }

        public string AdvancedSearchGenre
        {
            get => _advancedSearchGenre;
            set
            {
                _advancedSearchGenre = value;
                OnPropertyChanged(nameof(AdvancedSearchGenre));
            }
        }

        public string AdvancedSearchIsbn
        {
            get => _advancedSearchIsbn;
            set
            {
                _advancedSearchIsbn = value;
                OnPropertyChanged(nameof(AdvancedSearchIsbn));
            }
        }

        public ICommand LoadBooksCommand { get; private set; }
        public ICommand AddBookCommand { get; private set; }
        public ICommand BookSelectedCommand { get; private set; }
        public ICommand ToggleAdvancedSearchCommand { get; private set; }
        public ICommand SearchCommand { get; private set; }
        public ICommand ClearAdvancedSearchCommand { get; private set; }

        public BookViewModel()
        {
            _bookService = new BookService();
            Books = new ObservableCollection<BookModel>();

            LoadBooksCommand = new RelayCommand(async param => await LoadBooksAsync());
            AddBookCommand = new RelayCommand(param => OpenAddBookWindow());
            BookSelectedCommand = new RelayCommand(param => OpenBookDetailsWindow(param as BookModel));
            ToggleAdvancedSearchCommand = new RelayCommand(param => ToggleAdvancedSearch());
            SearchCommand = new RelayCommand(async param => await ExecuteAdvancedSearch());
            ClearAdvancedSearchCommand = new RelayCommand(param => ClearAdvancedSearch());

            LoadBooksAsync().ConfigureAwait(false);
        }

        private void ToggleAdvancedSearch()
        {
            IsAdvancedSearchVisible = !IsAdvancedSearchVisible;

            if (!IsAdvancedSearchVisible)
            {
                // Jeśli zamykamy zaawansowane wyszukiwanie, czyścimy filtry
                ClearAdvancedSearch();
                // Przywracamy proste wyszukiwanie jeśli jest tekst
                FilterBooks();
            }
        }

        private void ClearAdvancedSearch()
        {
            AdvancedSearchTitle = string.Empty;
            AdvancedSearchAuthor = string.Empty;
            AdvancedSearchGenre = string.Empty;
            AdvancedSearchIsbn = string.Empty;
        }

        private async Task ExecuteAdvancedSearch()
        {
            IsLoading = true;
            NoResults = false;

            try
            {
                // Budujemy frazę wyszukiwania z poszczególnych pól
                string searchPhrase = BuildAdvancedSearchPhrase();

                if (string.IsNullOrWhiteSpace(searchPhrase))
                {
                    // Jeśli wszystkie pola są puste, pokaż wszystkie książki
                    Books = new ObservableCollection<BookModel>(_allBooks ?? new ObservableCollection<BookModel>());
                    UpdateResultsVisibility();
                    IsLoading = false;
                    return;
                }

                var books = await _bookService.SearchBooksAsync(searchPhrase);
                Books = new ObservableCollection<BookModel>(books);
                UpdateResultsVisibility();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"{LocalizationManager.Get("BooksLoadingError")}: {ex.Message}",
                    LocalizationManager.Get("Error"),
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        private string BuildAdvancedSearchPhrase()
        {
            // Łączymy wszystkie niepuste pola z zaawansowanego wyszukiwania
            var parts = new[]
            {
                AdvancedSearchTitle,
                AdvancedSearchAuthor,
                AdvancedSearchGenre,
                AdvancedSearchIsbn
            }
            .Where(p => !string.IsNullOrWhiteSpace(p))
            .ToList();

            return string.Join(" ", parts);
        }

        public async Task LoadBooksAsync()
        {
            IsLoading = true;
            NoResults = false;

            try
            {
                var books = await _bookService.GetAllAsync();
                _allBooks = new ObservableCollection<BookModel>(books);
                Books = new ObservableCollection<BookModel>(_allBooks);
                UpdateResultsVisibility();
                _isInitialized = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"{LocalizationManager.Get("BooksLoadingError")}: {ex.Message}",
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
                    (o.Isbn?.ToLower().Contains(searchText) ?? false) ||
                    (o.Description?.ToLower().Contains(searchText) ?? false) ||
                    (o.GenreDisplay?.ToLower().Contains(searchText) ?? false)
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
            if (_allBooks == null) return;

            var bookToRemove = _allBooks.FirstOrDefault(b => b.Id == bookId);
            if (bookToRemove != null)
            {
                _allBooks.Remove(bookToRemove);
                FilterBooks();
            }
        }

        private void OpenAddBookWindow()
        {
            try { 
            var addBookWindow = new AddBookWindow();
            addBookWindow.ShowDialog();


                // Po zamknięciu okna odświeżamy listę książek
                LoadBooksAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Błąd podczas otwierania okna dodawania książki: {ex.Message}",
                    "Błąd",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void OpenBookDetailsWindow(BookModel selectedBook)
        {
            if (selectedBook != null)
            {
                var bookDetailsWindow = new BookDetailsWindow(selectedBook, this);
                bookDetailsWindow.ShowDialog();
            }
        }

        public void UpdateBookInList(BookModel updatedBook)
        {
            if (_allBooks == null || updatedBook == null) return;

            var bookInAllBooks = _allBooks.FirstOrDefault(b => b.Id == updatedBook.Id);
            var bookInBooks = Books.FirstOrDefault(b => b.Id == updatedBook.Id);

            if (bookInAllBooks != null)
            {
                int indexInAllBooks = _allBooks.IndexOf(bookInAllBooks);
                _allBooks[indexInAllBooks] = updatedBook;
            }

            if (bookInBooks != null)
            {
                int indexInBooks = Books.IndexOf(bookInBooks);
                Books[indexInBooks] = updatedBook;
            }

            OnPropertyChanged(nameof(Books));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}