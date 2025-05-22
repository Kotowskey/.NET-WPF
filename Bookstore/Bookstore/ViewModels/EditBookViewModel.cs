using Bookstore.Models;
using Bookstore.Services;
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
    public class EditBookViewModel : INotifyPropertyChanged
    {
        private readonly BookService _bookService;
        private BookModel _bookToEdit;
        private readonly BookDetailsViewModel _bookDetailsViewModel;

        #region Properties
        private string _title;
        public string Title
        {
            get => _title;
            set { _title = value; OnPropertyChanged(nameof(Title)); }
        }

        private string _description;
        public string Description
        {
            get => _description;
            set { _description = value; OnPropertyChanged(nameof(Description)); }
        }

        private string _isbn;
        public string Isbn
        {
            get => _isbn;
            set { _isbn = value; OnPropertyChanged(nameof(Isbn)); }
        }

        private string _publicationYear;
        public string PublicationYear
        {
            get => _publicationYear;
            set { _publicationYear = value; OnPropertyChanged(nameof(PublicationYear)); }
        }

        private string _publisherName;
        public string PublisherName
        {
            get => _publisherName;
            set { _publisherName = value; OnPropertyChanged(nameof(PublisherName)); }
        }

        private bool _isNewPublisher;
        public bool IsNewPublisher
        {
            get => _isNewPublisher;
            set { _isNewPublisher = value; OnPropertyChanged(nameof(IsNewPublisher)); }
        }

        private string _newPublisherName;
        public string NewPublisherName
        {
            get => _newPublisherName;
            set { _newPublisherName = value; OnPropertyChanged(nameof(NewPublisherName)); }
        }

        private string _newPublisherCountry;
        public string NewPublisherCountry
        {
            get => _newPublisherCountry;
            set { _newPublisherCountry = value; OnPropertyChanged(nameof(NewPublisherCountry)); }
        }

        private string _seriesName;
        public string SeriesName
        {
            get => _seriesName;
            set { _seriesName = value; OnPropertyChanged(nameof(SeriesName)); }
        }

        private bool _isNewSeries;
        public bool IsNewSeries
        {
            get => _isNewSeries;
            set { _isNewSeries = value; OnPropertyChanged(nameof(IsNewSeries)); }
        }

        private string _newSeriesName;
        public string NewSeriesName
        {
            get => _newSeriesName;
            set { _newSeriesName = value; OnPropertyChanged(nameof(NewSeriesName)); }
        }

        private string _newSeriesDescription;
        public string NewSeriesDescription
        {
            get => _newSeriesDescription;
            set { _newSeriesDescription = value; OnPropertyChanged(nameof(NewSeriesDescription)); }
        }

        public ObservableCollection<GenreViewModel> Genres { get; } = new ObservableCollection<GenreViewModel>();
        public ObservableCollection<AuthorViewModel> Authors { get; } = new ObservableCollection<AuthorViewModel>();
        public ObservableCollection<string> AvailablePublishers { get; } = new ObservableCollection<string>();
        public ObservableCollection<string> AvailableSeries { get; } = new ObservableCollection<string>();

        private string _newGenreText;
        public string NewGenreText
        {
            get => _newGenreText;
            set { _newGenreText = value; OnPropertyChanged(nameof(NewGenreText)); }
        }

        private string _newAuthorText;
        public string NewAuthorText
        {
            get => _newAuthorText;
            set { _newAuthorText = value; OnPropertyChanged(nameof(NewAuthorText)); }
        }
        #endregion

        #region Commands
        public ICommand AddGenreCommand { get; private set; }
        public ICommand AddAuthorCommand { get; private set; }
        public ICommand SaveChangesCommand { get; private set; }
        #endregion

        public EditBookViewModel(BookModel book, BookDetailsViewModel bookDetailsViewModel)
        {
            _bookService = new BookService();
            _bookToEdit = book;
            _bookDetailsViewModel = bookDetailsViewModel;
            SaveChangesCommand = new RelayCommand(async param => await UpdateBookAsync());

            Title = book.Title;
            Description = book.Description;
            Isbn = book.Isbn;
            PublicationYear = book.PublicationYear;

            if (book.Publisher != null)
            {
                IsNewPublisher = false;
                PublisherName = book.Publisher.Name;
                NewPublisherName = book.Publisher.Name;
                NewPublisherCountry = book.Publisher.Country;
            }
            else
            {
                IsNewPublisher = true;
            }

            if (book.Series != null)
            {
                IsNewSeries = false;
                SeriesName = book.Series.Name;
                NewSeriesName = book.Series.Name;
                NewSeriesDescription = book.Series.Description;
            }
            else
            {
                IsNewSeries = true;
            }

            AddGenreCommand = new RelayCommand(param => AddGenre(param as string));
            AddAuthorCommand = new RelayCommand(param => AddAuthor(param as string));
            SaveChangesCommand = new RelayCommand(async param => await UpdateBookAsync());

            Task.Run(async () => await LoadAvailableDataAsync());
        }

        private async Task LoadAvailableDataAsync()
        {
            try
            {
                var authors = await _bookService.GetAllAuthorAsync();
                var genres = await _bookService.GetAllGenreAsync();
                var publishers = await _bookService.GetAllPublisherAsync();
                var series = await _bookService.GetAllSeriesAsync();

                Authors.Clear();
                foreach (var author in authors)
                {
                    var authorVM = new AuthorViewModel
                    {
                        Name = author.Name,
                        IsSelected = _bookToEdit.Author.Any(a => a.Name == author.Name)
                    };
                    Authors.Add(authorVM);
                }

                Genres.Clear();
                foreach (var genre in genres)
                {
                    var genreVM = new GenreViewModel
                    {
                        Name = genre.Name,
                        IsSelected = _bookToEdit.Genre.Any(g => g.Name == genre.Name)
                    };
                    Genres.Add(genreVM);
                }

                AvailablePublishers.Clear();
                foreach (var publisher in publishers)
                {
                    AvailablePublishers.Add(publisher.Name);
                }

                AvailableSeries.Clear();
                foreach (var s in series)
                {
                    AvailableSeries.Add(s.Name);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd podczas ładowania danych: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AddGenre(string genreName)
        {
            if (!string.IsNullOrWhiteSpace(genreName))
            {
                var trimmedName = genreName.Trim();
                var existing = Genres.FirstOrDefault(g => g.Name.Equals(trimmedName, StringComparison.OrdinalIgnoreCase));
                if (existing == null)
                {
                    Genres.Add(new GenreViewModel { Name = trimmedName, IsSelected = true });
                }
                else
                {
                    existing.IsSelected = true;
                }
                NewGenreText = string.Empty;
                OnPropertyChanged(nameof(NewGenreText));
            }
        }

        private void AddAuthor(string authorName)
        {
            if (!string.IsNullOrWhiteSpace(authorName))
            {
                var trimmedName = authorName.Trim();
                var existing = Authors.FirstOrDefault(a => a.Name.Equals(trimmedName, StringComparison.OrdinalIgnoreCase));
                if (existing == null)
                {
                    Authors.Add(new AuthorViewModel { Name = trimmedName, IsSelected = true });
                }
                else
                {
                    existing.IsSelected = true;
                }
                NewAuthorText = string.Empty;
                OnPropertyChanged(nameof(NewAuthorText));
            }
        }

        private async Task UpdateBookAsync()
        {
            if (string.IsNullOrWhiteSpace(Title) || string.IsNullOrWhiteSpace(Isbn))
            {
                MessageBox.Show("Tytuł i ISBN są wymagane.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var updatedBook = new BookModel
            {
                Id = _bookToEdit.Id,
                Title = Title,
                Description = Description,
                Isbn = Isbn,
                PublicationYear = PublicationYear,
                Publisher = IsNewPublisher
                    ? new PublisherModel { Name = NewPublisherName, Country = NewPublisherCountry }
                    : new PublisherModel { Name = PublisherName },
                Series = IsNewSeries
                    ? new SeriesModel { Name = NewSeriesName, Description = NewSeriesDescription }
                    : new SeriesModel { Name = SeriesName },
                Genre = Genres.Where(g => g.IsSelected).Select(g => new GenreModel { Name = g.Name }).ToList(),
                Author = Authors.Where(a => a.IsSelected).Select(a => new AuthorModel { Name = a.Name }).ToList()
            };

            try
            {
                var result = await _bookService.EditBookAsync(_bookToEdit.Id, updatedBook);
                MessageBox.Show("Zmiany zostały zapisane pomyślnie!", "Sukces", MessageBoxButton.OK, MessageBoxImage.Information);
                Application.Current.Windows.OfType<EditBookWindow>().FirstOrDefault()?.Close();
                await _bookDetailsViewModel.RefreshBookDetailsAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd podczas zapisywania zmian: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}