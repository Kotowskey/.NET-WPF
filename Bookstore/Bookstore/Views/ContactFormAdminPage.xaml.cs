using Bookstore.Models;
using Bookstore.ViewModels;
using Bookstore.Translation;
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
    /// Logika interakcji dla klasy ContactFormAdminPage.xaml
    /// </summary>
    public partial class ContactFormAdminPage : UserControl
    {
        public ContactFormViewModel ViewModel { get; set; }

        public ContactFormAdminPage()
        {
            InitializeComponent();
            ViewModel = new ContactFormViewModel();
            DataContext = ViewModel;

            Loaded += ContactFormPage_Loaded;
        }

        private async void ContactFormPage_Loaded(object sender, RoutedEventArgs e)
        {
            await ViewModel.LoadMessagesAsync();
        }

        private async void ArchiveButton_Click(object sender, RoutedEventArgs e)
        {
            if (MessagesDataGrid.SelectedItem is ContactFormModel selectedMessage)
            {
                var result = MessageBox.Show(LocalizationManager.Get("ConfirmArchive"), LocalizationManager.Get("Confirm"), MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    await ViewModel.ArchiveMessageAsync(selectedMessage);
                }
            }
            else
            {
                MessageBox.Show(LocalizationManager.Get("SelectMessage"));
            }
        }
    }
}
