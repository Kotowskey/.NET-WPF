using Bookstore.Models;
using Bookstore.Models.Enums;
using Bookstore.Services;
using Bookstore.SignalRHub;
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
    public class MyOfferViewModel : INotifyPropertyChanged
    {
        private readonly ConnectionHub _connectionHub;
        private readonly ApiService _apiService;
        private readonly BookService _bookService;
        private Guid _currentUserId;

        private ObservableCollection<Offer> _offers;
        private ObservableCollection<Offer> _allOffers;
        public ObservableCollection<OfferItemViewModel> DraftOffers { get; } = new ObservableCollection<OfferItemViewModel>();
        public ObservableCollection<OfferItemViewModel> PublicOffers { get; } = new ObservableCollection<OfferItemViewModel>();
        public ObservableCollection<OfferItemViewModel> PrivateOffers { get; } = new ObservableCollection<OfferItemViewModel>();
        public ObservableCollection<OfferItemViewModel> OrderedOffers { get; } = new ObservableCollection<OfferItemViewModel>();
        
        private string _searchText;
        private bool _isLoading;
        private bool _noResults;

        public ObservableCollection<Offer> Offers
        {
            get => _offers;
            set { _offers = value; OnPropertyChanged(nameof(Offers)); }
        }

        public string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;
                OnPropertyChanged(nameof(SearchText));
                FilterOffers();
            }
        }

        public bool IsLoading
        {
            get => _isLoading;
            set { _isLoading = value; OnPropertyChanged(nameof(IsLoading)); }
        }

        public bool NoResults
        {
            get => _noResults;
            set { _noResults = value; OnPropertyChanged(nameof(NoResults)); }
        }

        public ICommand LoadOffersCommand { get; }
        public ICommand AddOfferCommand { get; }
        public ICommand OfferSelectedCommand { get; }
        public ICommand SearchCommand { get; }

        public MyOfferViewModel(ConnectionHub connectionHub)
        {
            _apiService = new ApiService();
            _bookService = new BookService();
            _connectionHub = connectionHub;

            Offers = new ObservableCollection<Offer>();
            _allOffers = new ObservableCollection<Offer>();

            LoadOffersCommand = new RelayCommand(async _ => await LoadOffersAsync());
            AddOfferCommand = new RelayCommand(_ => OpenAddOfferWindow());
            OfferSelectedCommand = new RelayCommand(param => OpenOfferDetails(param as Offer));
            SearchCommand = new RelayCommand(_ => FilterOffers());
            LoadUserIdAsync();
        }

        private async void LoadUserIdAsync()
        {
            _currentUserId = await _connectionHub.GetUserId();
            await LoadOffersAsync();
        }

        private async Task LoadOffersAsync()
        {
            try
            {
                IsLoading = true;
                var list = await _apiService.GetByRequesterAsync(_currentUserId);

                DraftOffers.Clear();
                PublicOffers.Clear();
                PrivateOffers.Clear();
                OrderedOffers.Clear();

                foreach (var o in list)
                {
                    var vm = new OfferItemViewModel(o, _apiService, _bookService);
                    tasks.Add(vm.LoadImageAsync());
                    tasks.Add(vm.BookLoadTask);
                    switch (o.OfferStateEnum)
                    {
                        case (int)OfferStateEnum.DraftOffer: DraftOffers.Add(vm); break;
                        case (int)OfferStateEnum.PublicOffer: PublicOffers.Add(vm); break;
                        case (int)OfferStateEnum.PrivateOffer: PrivateOffers.Add(vm); break;
                        case (int)OfferStateEnum.OrderedOffer: OrderedOffers.Add(vm); break;
                    }
                }
                NoResults = DraftOffers.Count + PublicOffers.Count + PrivateOffers.Count + OrderedOffers.Count == 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void FilterOffers()
        {
            if (_allOffers == null) return;

            if (string.IsNullOrWhiteSpace(SearchText))
            {
                Offers = new ObservableCollection<Offer>(_allOffers);
            }
            else
            {
                var txt = SearchText.ToLower();
                var filtered = _allOffers
                    .Where(o => o.Name?.ToLower().Contains(txt) == true ||
                                o.Description?.ToLower().Contains(txt) == true)
                    .ToList();
                Offers = new ObservableCollection<Offer>(filtered);
            }
            UpdateResultsVisibility();
        }

        private void UpdateResultsVisibility()
        {
            NoResults = Offers == null || Offers.Count == 0;
        }

        private void OpenAddOfferWindow()
        {
            var wnd = new AddOfferWindow(_apiService, _bookService, _currentUserId);
            if (wnd.ShowDialog() == true)
            {
                _ = LoadOffersAsync();
            }
        }

        private void OpenOfferDetails(Offer selected)
        {
            //if (selected == null) return;
            //var wnd = new OfferDetailsWindow(selected, this);
            //wnd.ShowDialog();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string prop) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
    }
}
