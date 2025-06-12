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
using System.Windows.Shapes;
using System.Net.Http;
using System.Net.Http.Json;
using Bookstore.Services;
using Bookstore.Models;
using Bookstore.Translation;

namespace Bookstore.Views
{
    public partial class ContactForm : Window
    {
        private readonly Guid _currentUserId;
        private readonly ContactFormService _contactFormService = new ContactFormService();

        public ContactForm(Guid currentUserId)
        {
            InitializeComponent();

            _currentUserId = currentUserId;

            AddText(SubjectBox, null);
            AddText(MessageBox, null);
        }

        private async void SubmitForm_Click(object sender, RoutedEventArgs e)
        {
            var form = new ContactFormModel
            {
                Subject = SubjectBox.Text,
                Message = MessageBox.Text,
                SubmitterId = _currentUserId
            };
            var success = await _contactFormService.SubmitContactFormAsync(form);

            if (success)
            {
                System.Windows.MessageBox.Show("Formularz został wysłany pomyślnie.", "Sukces", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                System.Windows.MessageBox.Show("Nie udało się wysłać formularza. Spróbuj ponownie.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void RemoveText(object sender, RoutedEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            if (tb.Text == LocalizationManager.Get("SubjectPlaceholder") || tb.Text == LocalizationManager.Get("MessagePlaceholder") || tb.Text.Contains("GUID"))
            {
                tb.Text = "";
                tb.Foreground = Brushes.Black;
            }
        }

        private void AddText(object sender, RoutedEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            if (string.IsNullOrWhiteSpace(tb.Text))
            {
                if (tb == SubjectBox) tb.Text = LocalizationManager.Get("SubjectPlaceholder");
                else if (tb == MessageBox) tb.Text = LocalizationManager.Get("MessagePlaceholder");
                tb.Foreground = Brushes.Gray;
            }
        }
    }
}
