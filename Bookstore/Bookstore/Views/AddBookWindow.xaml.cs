using Bookstore.Models;
using Bookstore.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http.Json;
using System.Text.Json;
using System.Windows;
using Bookstore.ViewModels;
using System.Windows.Controls;
using System.Windows.Input;

namespace Bookstore.Views
{
    public partial class AddBookWindow : Window
    {
        public AddBookWindow()
        {
            InitializeComponent();
            DataContext = new AddBookViewModel();
        }
    }
}
