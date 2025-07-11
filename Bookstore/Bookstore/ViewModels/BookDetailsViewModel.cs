﻿using Bookstore.Models;
using Bookstore.Services;
using Bookstore.Translation;
using Bookstore.Views;
using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Bookstore.ViewModels
{
    public class BookDetailsViewModel : INotifyPropertyChanged
    {
        private readonly BookService _bookService;
        private readonly BookViewModel _parentViewModel;
        private BookModel _book;

        public BookModel Book
        {
            get => _book;
            set
            {
                _book = value;
                OnPropertyChanged(nameof(Book));
            }
        }

        public ICommand DeleteBookCommand { get; private set; }
        public ICommand CloseWindowCommand { get; private set; }
        public ICommand EditBookCommand { get; private set; }

        public BookDetailsViewModel(BookModel book, BookViewModel parentViewModel)
        {
            _bookService = new BookService();
            _parentViewModel = parentViewModel;
            Book = book;

            DeleteBookCommand = new RelayCommand(async param => await DeleteBookAsync());
            CloseWindowCommand = new RelayCommand(param => CloseWindow(param as Window));
            EditBookCommand = new RelayCommand(param => OpenEditBookWindowAsync());
        }

        private async Task DeleteBookAsync()
        {
            var result = MessageBox.Show(
                $"Czy na pewno chcesz usunąć książkę '{Book.Title}'?",
                "Potwierdzenie usunięcia",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    bool success = await _bookService.DeletedAsync(Book.Id);

                    if (success)
                    {
                        _parentViewModel.RemoveBookFromList(Book.Id);
                        MessageBox.Show(LocalizationManager.Get("DeleteBookSuccess"), LocalizationManager.Get("Success"), MessageBoxButton.OK, MessageBoxImage.Information);

                        Application.Current.Windows[1]?.Close();
                    }
                    else
                    {
                        MessageBox.Show("Nie udało się usunąć książki.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Błąd podczas usuwania książki: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void CloseWindow(Window window)
        {
            window?.Close();
        }

        public async Task RefreshBookDetailsAsync()
        {
            try
            {
                Book = await _bookService.GetByIdAsync(Book.Id);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd podczas odświeżania danych: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task OpenEditBookWindowAsync()
        {
            var editBookWindow = new EditBookWindow(Book, this);
            editBookWindow.ShowDialog();
            await RefreshBookDetailsAsync();
            _parentViewModel.UpdateBookInList(Book);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}