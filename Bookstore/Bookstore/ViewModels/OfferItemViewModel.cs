using Bookstore.Models;
using Bookstore.Services;
using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Bookstore.ViewModels
{
    public class OfferItemViewModel : INotifyPropertyChanged
    {
        private readonly ApiService _apiService;
        private readonly string _baseUrl = "https://localhost:5257"; // dostosuj do swojego API

        public Offer Model { get; }
        public string Name => Model.Name;
        public string Description => Model.Description;

        private BitmapImage _imageSource;
        public BitmapImage ImageSource
        {
            get => _imageSource;
            set { _imageSource = value; OnPropertyChanged(nameof(ImageSource)); }
        }

        public OfferItemViewModel(Offer offer, ApiService apiService)
        {
            Model = offer;
            _apiService = apiService;
            _ = LoadImageAsync();
        }

        private async Task LoadImageAsync()
        {
            if (Model.FileId.HasValue)
            {
                try
                {
                    // pobierz FileModel lub DTO z backendu
                    var file = await _apiService.GetFileByIdAsync(Model.FileId.Value);
                    if (file != null && !string.IsNullOrEmpty(file.Source))
                    {
                        ImageSource = new BitmapImage(new Uri(_baseUrl + file.Source, UriKind.Absolute));
                        return;
                    }
                }
                catch (Exception ex) { 
                    Console.WriteLine($"Błąd podczas pobierania obrazu: {ex.Message}"); 
                }
            }

            // domyślny obrazek z pliku w projekcie
            var defaultUri = new Uri("pack://application:,,,/Resources/default.png");
            ImageSource = new BitmapImage(defaultUri);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
