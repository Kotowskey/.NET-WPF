using Bookstore.Models;
using Bookstore.Services;
using Bookstore.Views;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Bookstore.ViewModels
{
    public class OfferDetailsViewModel : INotifyPropertyChanged
    {
        private readonly OfferItemViewModel _offerViewModel;
        private readonly MyOfferViewModel _parentViewModel;
        private readonly BookService _bookService;

        public OfferItemViewModel Offer => _offerViewModel;

        public ICommand LoadDetailsCommand { get; }
        public ICommand OpenBookDetailsCommand { get; }
        public ICommand CloseWindowCommand { get; }

        public OfferDetailsViewModel(OfferItemViewModel offerViewModel, MyOfferViewModel parentViewModel)
        {
            _offerViewModel = offerViewModel;
            _parentViewModel = parentViewModel;
            _bookService = new BookService();

            LoadDetailsCommand = new RelayCommand(async _ => await LoadDetailsAsync());
            OpenBookDetailsCommand = new RelayCommand(_ => OpenBookDetails());
            CloseWindowCommand = new RelayCommand(param => CloseWindow(param as Window));
        }

        private async Task LoadDetailsAsync()
        {
            // Ensure book and image data are loaded
            await Task.WhenAll(_offerViewModel.BookLoadTask, _offerViewModel.ImageLoadTask);
            OnPropertyChanged(nameof(Offer));
        }

        private void OpenBookDetails()
        {
            //if (_offerViewModel.Book != null)
            //{
            //    var bookDetailsWindow = new BookDetailsWindow(_offerViewModel.Book, null); // No parent ViewModel needed here
            //    bookDetailsWindow.ShowDialog();
            //}
            //else
            //{
            //    MessageBox.Show("Nie udało się załadować szczegółów książki.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            //}
        }

        private void CloseWindow(Window window)
        {
            window?.Close();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}