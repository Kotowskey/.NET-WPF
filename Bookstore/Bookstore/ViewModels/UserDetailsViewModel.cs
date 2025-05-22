using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bookstore.Models;

namespace Bookstore.ViewModels
{
    public class UserDetailsViewModel : INotifyPropertyChanged
    {
        public UserModel User { get; set; }
        private UserViewModel _parent;

        public UserDetailsViewModel(UserModel user, UserViewModel parent)
        {
            User = user;
            _parent = parent;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        // Implementacja property changed
    }
}
