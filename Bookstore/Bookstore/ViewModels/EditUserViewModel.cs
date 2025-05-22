using Bookstore.Models;
using System.ComponentModel;

namespace Bookstore.ViewModels
{
    public class EditUserViewModel : INotifyPropertyChanged
    {
        public UserModel User { get; set; }
        private UserDetailsViewModel _parent;

        public EditUserViewModel(UserModel user, UserDetailsViewModel parent)
        {
            User = user;
            _parent = parent;
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
