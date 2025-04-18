using Bookstore.Models;
using Bookstore.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http.Json;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Bookstore.Views
{
    public partial class AddBookWindow : Window
    {
        private readonly BookService _bookService;
        private List<AuthorModel> _availableAuthors;
        private List<GenreModel> _availableGenres;
        private List<PublisherModel> _availablePublishers;
        private List<SeriesModel> _availableSeries;
        private List<BookModel> _addBook;
        public ObservableCollection<string> Genres { get; set; } = new ObservableCollection<string>();
        public ObservableCollection<string> Authors { get; set; } = new ObservableCollection<string>();

        public AddBookWindow()
        {
            InitializeComponent();
            _bookService = new BookService();
            DataContext = this;
            Loaded += AddBookWindow_Loaded;
        }

        private async void AddBookWindow_Loaded(object sender, RoutedEventArgs e)
        {
            _availableAuthors = await _bookService.GetAllAuthorAsync();
            _availableGenres = await _bookService.GetAllGenreAsync();
            _availablePublishers = await _bookService.GetAllPublisherAsync();
            _availableSeries = await _bookService.GetAllSeriesAsync();

            PublisherComboBox.ItemsSource = _availablePublishers.Select(p => p.Name).ToList();
            SeriesComboBox.ItemsSource = _availableSeries.Select(s => s.Name).ToList();
            GenreListBox.ItemsSource = _availableGenres.Select(g => g.Name).ToList();
            AuthorListBox.ItemsSource = _availableAuthors.Select(a => a.Name).ToList();

            Genres.Clear();
            foreach (var genre in _availableGenres)
            {
                Genres.Add(genre.Name);
            }

            Authors.Clear();
            foreach (var author in _availableAuthors)
            {
                Authors.Add(author.Name);
            }
        }

        private void NewGenreBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && !string.IsNullOrWhiteSpace(NewGenreBox.Text))
            {
                var newItem = NewGenreBox.Text.Trim();
                if (!Genres.Contains(newItem))
                {
                    Genres.Add(newItem);
                }

                GenreListBox.SelectedItems.Add(newItem);

                NewGenreBox.Clear();
            }
        }

        private void NewAuthorBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && !string.IsNullOrWhiteSpace(NewAuthorBox.Text))
            {
                var newItem = NewAuthorBox.Text.Trim();
                if (!Authors.Contains(newItem))
                {
                    Authors.Add(newItem);
                }

                AuthorListBox.SelectedItems.Add(newItem);
                NewAuthorBox.Clear();
            }
        }

        private async void SubmitButton_Click(object sender, RoutedEventArgs e)
        {
            var book = new BookModel
            {
                Title = TitleBox.Text,
                Description = DescriptionBox.Text,
                Isbn = IsbnBox.Text,
                PublicationYear = PublicationYearBox.Text,
                Publisher = new PublisherModel
                {
                    Name = PublisherComboBox.Text,
                    Country = PublisherCountryBox.Text
                },
                Series = new SeriesModel
                {
                    Name = SeriesComboBox.Text,
                    Description = SeriesDescriptionBox.Text
                },
                Genre = GenreListBox.SelectedItems.Cast<string>()
                .Select(name => new GenreModel { Name = name })
                .ToList(),
                Author = AuthorListBox.SelectedItems.Cast<string>()
                .Select(name => new AuthorModel { Name = name })
                .ToList()
            };

            try
            {
                await _bookService.AddBookAsync(book);
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Wyjątek: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
