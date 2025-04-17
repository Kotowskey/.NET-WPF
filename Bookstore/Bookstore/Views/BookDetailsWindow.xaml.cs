using Bookstore.Models;
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

namespace Bookstore.Views
{
    /// <summary>
    /// Logika interakcji dla klasy BookDetailsWindow.xaml
    /// </summary>
    public partial class BookDetailsWindow : Window
    {
        public BookDetailsWindow(BookModel selectedBook)
        {
            InitializeComponent();

            BookTitle.Text = selectedBook.Title;
            BookAuthor.Text = selectedBook.AuthorDisplay;
            BookDescription.Text = selectedBook.Description;
            BookPublicationYear.Text = selectedBook.PublicationYear;
            BookPublisher.Text = selectedBook.PublisherDisplay;
            BookGenre.Text = selectedBook.GenreDisplay;
            BookSeries.Text = selectedBook?.SeriesDisplay;
            BookIsbn.Text = selectedBook.Isbn;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
