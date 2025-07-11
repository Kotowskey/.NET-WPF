﻿using Bookstore.Models;
using Bookstore.Services;
using Bookstore.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Bookstore.Views
{
    /// <summary>
    /// Interaction logic for OffersPage.xaml
    /// </summary>
    public partial class OffersPage : UserControl
    {
        private readonly ApiService _apiService;
        private List<Offer> _allOffers;
        private CartService _cartService;
        private Guid _currentUserId;

        public OffersPage()
        {
            InitializeComponent();
            _apiService = new ApiService();

            SearchBox.TextChanged += SearchBox_TextChanged;

            // Load offers when the page is loaded
            Loaded += async (s, e) => {
                _currentUserId = await App.Auth.GetUserIdAsync();
                _cartService = new CartService(_currentUserId);
                await LoadOffers();
            };
        }

        private async Task LoadOffers()
        {
            LoadingCard.Visibility = Visibility.Visible;
            NoResultsText.Visibility = Visibility.Collapsed;
            OffersListView.Visibility = Visibility.Collapsed;

            try
            {
                _allOffers = await _apiService.GetPublicOffersAsync();

                var cartIds = _cartService.LoadCart();
                foreach (var o in _allOffers)
                    o.IsInCart = cartIds.Contains(o);

                UpdateOffersDisplay(_allOffers);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Nie można załadować ofert: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                LoadingCard.Visibility = Visibility.Collapsed;
                OffersListView.Visibility = Visibility.Visible;
            }
        }

        private void UpdateOffersDisplay(List<Offer> offers)
        {
            OffersListView.ItemsSource = offers;

            if (offers == null || offers.Count == 0)
            {
                NoResultsText.Visibility = Visibility.Visible;
                OffersListView.Visibility = Visibility.Collapsed;
            }
            else
            {
                NoResultsText.Visibility = Visibility.Collapsed;
                OffersListView.Visibility = Visibility.Visible;
            }
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_allOffers == null) return;

            var searchText = SearchBox.Text.ToLower();
            if (string.IsNullOrWhiteSpace(searchText))
            {
                UpdateOffersDisplay(_allOffers);
                return;
            }

            var filteredOffers = _allOffers.Where(o =>
                (o.Name?.ToLower().Contains(searchText) ?? false) ||
                (o.Description?.ToLower().Contains(searchText) ?? false)).ToList();

            UpdateOffersDisplay(filteredOffers);
        }

        private async void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            await LoadOffers();
        }

        private void OffersListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (OffersListView.SelectedItem is Offer selectedOffer)
            {
                MessageBox.Show($"Wybrano ofertę: {selectedOffer.Name}");
                // Here you would navigate to a detail page or show a detail dialog
            }
        }

        private void CartButton_Click(object sender, RoutedEventArgs e)
        {
            var offer = (sender as Button)?.Tag as Offer;
            if (offer == null) return;

            if (offer.IsInCart)
            {
                _cartService.Remove(offer);
                offer.IsInCart = false;
            }
            else
            {
                _cartService.Add(offer);
                offer.IsInCart = true;
            }

            OffersListView.Items.Refresh();
        }
    }
}