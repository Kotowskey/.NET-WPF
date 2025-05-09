using Bookstore.Models;
using Bookstore.Services;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Bookstore.ViewModels
{
    public class AddBookViewModel : INotifyPropertyChanged
    {
        private readonly BookService _bookService;

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

        private string _publisherCountry;
        public string PublisherCountry
        {
            get => _publisherCountry;
            set { _publisherCountry = value; OnPropertyChanged(nameof(PublisherCountry)); }
        }

        private string _seriesName;
        public string SeriesName
        {
            get => _seriesName;
            set { _seriesName = value; OnPropertyChanged(nameof(SeriesName)); }
        }

        private string _seriesDescription;
        public string SeriesDescription
        {
            get => _seriesDescription;
            set { _seriesDescription = value; OnPropertyChanged(nameof(SeriesDescription)); }
        }

        public ObservableCollection<GenreViewModel> Genres { get; } = new ObservableCollection<GenreViewModel>();
        public ObservableCollection<AuthorViewModel> Authors { get; } = new ObservableCollection<AuthorViewModel>();
        public ObservableCollection<string> AvailablePublishers { get; } = new ObservableCollection<string>();
        public ObservableCollection<string> AvailableSeries { get; } = new ObservableCollection<string>();

        private bool _isBookAdded;
        public bool IsBookAdded
        {
            get => _isBookAdded;
            private set { _isBookAdded = value; OnPropertyChanged(nameof(IsBookAdded)); }
        }
        #endregion

        #region Commands
        public ICommand AddGenreCommand { get; private set; }
        public ICommand AddAuthorCommand { get; private set; }
        public ICommand AddBookCommand { get; private set; }
        #endregion

        public AddBookViewModel()
        {
            _bookService = new BookService();
            AddGenreCommand = new RelayCommand(param => AddGenre(param as string));
            AddAuthorCommand = new RelayCommand(param => AddAuthor Spill(param as string));
            AddBookCommand = new RelayCommand(async param => await AddBookAsync());
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
                    Authors.Add(new AuthorViewModel { Name = author.Name });
                }

                Genres.Clear();
                foreach (var genre in genres)
                {
                    Genres.Add(new GenreViewModel { Name = genre.Name });
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
                var existing = Genres.FirstOrDefault(g => g.Name == trimmedName);
                if (existing == null)
                {
                    Genres.Add(new GenreViewModel { Name = trimmedName, IsSelected = true });
                }
                else
                {
                    existing.IsSelected = true;
                }
            }
        }

        private void AddAuthor(string authorName)
        {
            if (!string.IsNullOrWhiteSpace(authorName))
            {
                var trimmedName = authorName.Trim();
                var existing = Authors.FirstOrDefault(a => a.Name == trimmedName);
                if (existing == null)
                {
                    Authors.Add(new AuthorViewModel { Name = trimmedName, IsSelected = true });
                }
                else
                {
                    existing.IsSelected = true;
                }
            }
        }

        private async void AddBookAsync()
        {
            var book = new BookModel
            {
                Title = Title,
                Description = Description,
                Isbn = Isbn,
                PublicationYear = PublicationYear,
                Publisher = new PublisherModel
                {
                    Name = PublisherName,
                    Country = PublisherCountry
                },
                Series = new SeriesModel
                {
                    Name = SeriesName,
                    Description = SeriesDescription
                },
                Genre = Genres.Where(g => g.IsSelected).Select(g => new GenreModel { Name = g.Name }).ToList(),
                Author = Authors.Where(a => a.IsSelected).Select(a => new AuthorModel { Name = a.Name }).ToList()
            };

            try
            {
                await _bookService.AddBookAsync(book);
                IsBookAdded = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd podczas dodawania książki: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class GenreViewModel : INotifyPropertyChanged
    {
        private string _name;
        private bool _isSelected;

        public string Name
        {
            get => _name;
            set { _name = value; OnPropertyChanged(nameof(Name)); }
        }

        public bool IsSelected
        {
            get => _isSelected;
            set { _isSelected = value; OnPropertyChanged(nameof(IsSelected)); }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class AuthorViewModel : INotifyPropertyChanged
    {
        private string _name;
        private bool _isSelected;

        public string Name
        {
            get => _name;
            set { _name = value; OnPropertyChanged(nameof(Name)); }
        }

        public bool IsSelected
        {
            get => _isSelected;
            set { _isSelected = value; OnPropertyChanged(nameof(IsSelected)); }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}