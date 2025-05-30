using Bookstore.Models;
using Bookstore.Services;
using Bookstore.Views;
using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Bookstore.ViewModels
{
    public class AddOfferViewModel : INotifyPropertyChanged
    {
        private readonly ApiService _apiService;
        private readonly BookService _bookService;
        private readonly Guid _requesterId;
        public event EventHandler OfferAdded;

        #region Properties
        private string _name;
        public string Name
        {
            get => _name;
            set { _name = value; OnPropertyChanged(nameof(Name)); }
        }

        private string _description;
        public string Description
        {
            get => _description;
            set { _description = value; OnPropertyChanged(nameof(Description)); }
        }

        private float _price;
        public float Price
        {
            get => _price;
            set { _price = value; OnPropertyChanged(nameof(Price)); }
        }

        private BookModel _selectedBook;
        public BookModel SelectedBook
        {
            get => _selectedBook;
            set { _selectedBook = value; OnPropertyChanged(nameof(SelectedBook)); }
        }

        private string _selectedFilePath;
        public string SelectedFilePath
        {
            get => _selectedFilePath;
            set { _selectedFilePath = value; OnPropertyChanged(nameof(SelectedFilePath)); }
        }

        private string _selectedFileName;
        public string SelectedFileName
        {
            get => _selectedFileName;
            set { _selectedFileName = value; OnPropertyChanged(nameof(SelectedFileName)); }
        }

        private bool _isDraft = true;
        public bool IsDraft
        {
            get => _isDraft;
            set { _isDraft = value; OnPropertyChanged(nameof(IsDraft)); }
        }

        private bool _isPublic;
        public bool IsPublic
        {
            get => _isPublic;
            set { _isPublic = value; OnPropertyChanged(nameof(IsPublic)); }
        }

        private bool _isPrivate;
        public bool IsPrivate
        {
            get => _isPrivate;
            set { _isPrivate = value; OnPropertyChanged(nameof(IsPrivate)); }
        }

        private bool _isOfferAdded;
        public bool IsOfferAdded
        {
            get => _isOfferAdded;
            private set { _isOfferAdded = value; OnPropertyChanged(nameof(IsOfferAdded)); }
        }

        public ObservableCollection<BookModel> Books { get; } = new ObservableCollection<BookModel>();
        #endregion

        #region Commands
        public ICommand SelectFileCommand { get; private set; }
        public ICommand AddOfferCommand { get; private set; }
        #endregion

        public AddOfferViewModel(ApiService apiService, BookService bookService, Guid requesterId)
        {
            _apiService = apiService;
            _bookService = bookService;
            _requesterId = requesterId;

            SelectFileCommand = new RelayCommand(_ => SelectFile());
            AddOfferCommand = new RelayCommand(async _ => await AddOfferAsync());

            Task.Run(async () => await LoadBooksAsync());
        }

        private async Task LoadBooksAsync()
        {
            try
            {
                var books = await _bookService.GetAllAsync();
                Books.Clear();
                foreach (var book in books)
                {
                    Books.Add(book);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd podczas ładowania książek: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SelectFile()
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "Obrazy (*.png;*.jpeg;*.jpg)|*.png;*.jpeg;*.jpg|Wszystkie pliki (*.*)|*.*",
                Title = "Wybierz plik do wgrania"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                SelectedFilePath = openFileDialog.FileName;
                SelectedFileName = System.IO.Path.GetFileName(SelectedFilePath);
            }
        }

        private async Task AddOfferAsync()
        {
            if (string.IsNullOrWhiteSpace(Name) || SelectedBook == null || Price <= 0)
            {
                MessageBox.Show("Nazwa, książka i cena są wymagane.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                int? fileId = null;
                if (!string.IsNullOrWhiteSpace(SelectedFilePath))
                {
                    var uploadedFile = await _apiService.UploadFileAsync(SelectedFilePath);
                    fileId = uploadedFile?.Id;
                }

                var offer = new Offer
                {
                    Name = Name,
                    Description = Description,
                    Price = Price,
                    BookId = SelectedBook.Id,
                    RequesterId = _requesterId,
                    FileId = fileId,
                    OfferStateEnum = IsDraft ? 0 : IsPublic ? 10 : 20,
                    CreatedDate = DateTime.Now
                };

                var addedOffer = await _apiService.AddOfferAsync(offer);
                if (addedOffer != null)
                {
                    IsOfferAdded = true;
                    MessageBox.Show("Oferta została dodana pomyślnie!", "Sukces", MessageBoxButton.OK, MessageBoxImage.Information);
                    //OfferAdded?.Invoke(this, EventArgs.Empty);
                    Application.Current.Windows.OfType<AddOfferWindow>().FirstOrDefault()?.Close();
                }
                else
                {
                    MessageBox.Show("Nie udało się dodać oferty.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd podczas dodawania oferty: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}