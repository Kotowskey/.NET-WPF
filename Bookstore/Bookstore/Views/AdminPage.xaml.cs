﻿using Bookstore.Translation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Bookstore.Views
{
    /// <summary>
    /// Interaction logic for AdminPage.xaml
    /// </summary>
    public partial class AdminPage : Window
    {
        public AdminPage()
        {
            InitializeComponent();
            this.Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // Inicjalizacja początkowego widoku
            HideAllViews();
            if (DashboardView != null)
            {
                DashboardView.Visibility = Visibility.Visible;
            }

            // Initialize StatsView with connection
            if (StatsView != null)
            {
                StatsView.Initialize();
            }
        }

        private void NavListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            HideAllViews();
            // Sprawdzamy każdy element przed zmianą jego widoczności
            if (DashboardItem != null && DashboardItem.IsSelected && DashboardView != null)
            {
                DashboardView.Visibility = Visibility.Visible;
            }
            else if (BooksItem != null && BooksItem.IsSelected && BooksView != null)
            {
                BooksView.Visibility = Visibility.Visible;
            }
            else if (OffersItem != null && OffersItem.IsSelected && OffersView != null)
            {
                OffersView.Visibility = Visibility.Visible;
            }
            else if (CustomersItem != null && CustomersItem.IsSelected && CustomersView != null)
            {
                CustomersView.Visibility = Visibility.Visible;
            }
            else if (OrdersItem != null && OrdersItem.IsSelected && OrdersView != null)
            {
                OrdersView.Visibility = Visibility.Visible;
            }
            else if (StatsItem != null && StatsItem.IsSelected && StatsView != null)
            {
                StatsView.Visibility = Visibility.Visible;
            }
            else if (ContactFormAdminItem != null && ContactFormAdminItem.IsSelected && ContactFormAdminView != null)
            {
                ContactFormAdminView.Visibility = Visibility.Visible;
            }
        }

        private void HideAllViews()
        {
            // Sprawdzamy każdy element przed zmianą jego widoczności
            if (DashboardView != null) DashboardView.Visibility = Visibility.Collapsed;
            if (BooksView != null) BooksView.Visibility = Visibility.Collapsed;
            if (OffersView != null) OffersView.Visibility = Visibility.Collapsed;
            if (CustomersView != null) CustomersView.Visibility = Visibility.Collapsed;
            if (OrdersView != null) OrdersView.Visibility = Visibility.Collapsed;
            if (StatsView != null) StatsView.Visibility = Visibility.Collapsed;
            if (ContactFormAdminView != null) ContactFormAdminView.Visibility = Visibility.Collapsed;
        }

        private void GoToOffers_Click(object sender, RoutedEventArgs e)
        {
            if (NavListView != null && OffersItem != null)
            {
                NavListView.SelectedItem = OffersItem;
            }
        }
        private void LanguageToggle_Checked(object sender, RoutedEventArgs e)
        {
            LocalizationManager.ChangeLanguage("en");
        }

        private void LanguageToggle_Unchecked(object sender, RoutedEventArgs e)
        {
            LocalizationManager.ChangeLanguage("pl");
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            _ = App.Auth.LogoutAsync();
            var loginWindow = new SingInUp();
            loginWindow.Show();
            this.Close();
        }

        private void GoToClient_Page(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}