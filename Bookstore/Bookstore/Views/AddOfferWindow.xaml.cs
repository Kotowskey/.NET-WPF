using Bookstore.Services;
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
using Bookstore.ViewModels;
using Bookstore.Models;

namespace Bookstore.Views
{
    /// <summary>
    /// Logika interakcji dla klasy AddOfferWindow.xaml
    /// </summary>
    public partial class AddOfferWindow : Window
    {
        public AddOfferWindow(ApiService apiService, BookService bookService, Guid requesterId, Offer offerToEdit)
        {
            InitializeComponent();
            DataContext = new AddOfferViewModel(apiService, bookService, requesterId, offerToEdit);
        }

        public AddOfferWindow(ApiService apiService, BookService bookService, Guid requesterId)
        {
            InitializeComponent();
            DataContext = new AddOfferViewModel(apiService, bookService, requesterId);
        }

    }
}
