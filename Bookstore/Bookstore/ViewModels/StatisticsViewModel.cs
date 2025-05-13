using Bookstore.Services;
using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Bookstore.ViewModels
{
    public class StatisticsViewModel : INotifyPropertyChanged
    {
        private readonly StatisticsService _statisticsService;
        private int _booksCount;
        private int _authorsCount;
        private int _genresCount;
        private int _publishersCount;
        private int _usersCount;
        private bool _isLoading;

        public int BooksCount
        {
            get => _booksCount;
            set
            {
                _booksCount = value;
                OnPropertyChanged(nameof(BooksCount));
            }
        }

        public int AuthorsCount
        {
            get => _authorsCount;
            set
            {
                _authorsCount = value;
                OnPropertyChanged(nameof(AuthorsCount));
            }
        }

        public int GenresCount
        {
            get => _genresCount;
            set
            {
                _genresCount = value;
                OnPropertyChanged(nameof(GenresCount));
            }
        }

        public int PublishersCount
        {
            get => _publishersCount;
            set
            {
                _publishersCount = value;
                OnPropertyChanged(nameof(PublishersCount));
            }
        }

        public int UsersCount
        {
            get => _usersCount;
            set
            {
                _usersCount = value;
                OnPropertyChanged(nameof(UsersCount));
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

        public ICommand RefreshCommand { get; private set; }

        public StatisticsViewModel()
        {
            _statisticsService = new StatisticsService();
            RefreshCommand = new RelayCommand(async param => await LoadStatisticsAsync());

            LoadStatisticsAsync().ConfigureAwait(false);
        }

        public async Task LoadStatisticsAsync()
        {
            IsLoading = true;

            try
            {
                var stats = await _statisticsService.GetAllStatisticsAsync();
                BooksCount = stats.BooksCount;
                AuthorsCount = stats.AuthorsCount;
                GenresCount = stats.GenresCount;
                PublishersCount = stats.PublishersCount;
                UsersCount = stats.UsersCount;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error loading statistics: {ex.Message}",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
