using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using Bookstore.Models.Enums;
using Bookstore.Models;
using Bookstore.Services;

namespace Bookstore.ViewModels
{
    public class ContactFormViewModel : INotifyPropertyChanged
    {
        private readonly ContactFormService _service;
        private ObservableCollection<ContactFormModel> _messages;
        private bool _isLoading;
        private bool _noResults;

        public ObservableCollection<ContactFormModel> Messages
        {
            get => _messages;
            set
            {
                _messages = value;
                OnPropertyChanged(nameof(Messages));
                NoResults = _messages == null || _messages.Count == 0;
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

        public bool NoResults
        {
            get => _noResults;
            set
            {
                _noResults = value;
                OnPropertyChanged(nameof(NoResults));
            }
        }

        public ContactFormViewModel()
        {
            _service = new ContactFormService(); // Załóżmy, że masz konstruktor bezparametrowy lub DI
            Messages = new ObservableCollection<ContactFormModel>();
        }

        public async Task LoadMessagesAsync()
        {
            try
            {
                IsLoading = true;
                NoResults = false;

                var messages = await _service.GetAllMessagesAsync();
                Messages = new ObservableCollection<ContactFormModel>(messages);                
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Błąd podczas ładowania wiadomości: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        public async Task ArchiveMessageAsync(ContactFormModel message)
        {
            if (message == null) return;

            try
            {
                IsLoading = true;
                message.StateEnum = StateEnum.Archived;

                var success = await _service.EditMessageAsync(message.Id, message);
                if (success != null)
                {
                    await LoadMessagesAsync();
                    MessageBox.Show("Wiadomość została oznaczona jako archiwalna.", "Sukces", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Nie udało się zarchiwizować wiadomości.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Błąd podczas archiwizacji wiadomości: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
