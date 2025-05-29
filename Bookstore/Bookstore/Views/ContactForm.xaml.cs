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

namespace Bookstore.Views
{
    public partial class ContactForm : Window
    {
        private readonly Guid _currentUserId;
        private readonly HttpClient _httpClient = new HttpClient();

        public ContactForm(Guid currentUserId)
        {
            InitializeComponent();

            _currentUserId = currentUserId;

            AddText(SubjectBox, null);
            AddText(MessageBox, null);
        }

        private async void SubmitForm_Click(object sender, RoutedEventArgs e)
        {
            var form = new
            {
                Subject = SubjectBox.Text,
                Message = MessageBox.Text,
                ReceiverId = Guid.Parse(ReceiverIdBox.Text),
                SubmitterId = _currentUserId,
                OrderId = (int?)null,
                OfferId = (int?)null,
                FileId = (int?)null,
                DateSubmitted = DateTime.Now,
                StateEnum = 0  // lub np. "Active" jeżeli enum to string
            };

            try
            {
                var response = await _httpClient.PostAsJsonAsync("http://localhost:5000/api/ContactForm/submit", form);

                if (response.IsSuccessStatusCode)
                {
                    System.Windows.MessageBox.Show("Formularz został wysłany pomyślnie.", "Sukces", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    System.Windows.MessageBox.Show("Nie udało się wysłać formularza. Spróbuj ponownie.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Wystąpił błąd: {ex.Message}", "Wyjątek", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void RemoveText(object sender, RoutedEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            if (tb.Text == "Temat" || tb.Text == "Wiadomość" || tb.Text.Contains("GUID"))
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
                if (tb == SubjectBox) tb.Text = "Temat";
                else if (tb == MessageBox) tb.Text = "Wiadomość";
                tb.Foreground = Brushes.Gray;
            }
        }
    }
}
