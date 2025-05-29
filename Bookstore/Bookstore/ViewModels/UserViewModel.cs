using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Bookstore.Models;
using Bookstore.Services;

namespace Bookstore.ViewModels
{
    public class UserViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<UserModel> Users = new ObservableCollection<UserModel>();
        private ObservableCollection<UserModel> _allUsers;

        private readonly UserService _userService = new UserService();
        private bool _isLoading = false;
        public bool IsLoading
        {
            get => _isLoading;
            set { _isLoading = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsLoading))); }
        }

        public UserViewModel()
        {
            _ = LoadUsersAsync();
        }

        public async Task LoadUsersAsync()
        {
            IsLoading = true;
            Users.Clear();
            var usersFromApi = await _userService.GetAllAsync();
            _allUsers = new ObservableCollection<UserModel>(usersFromApi);
            Users = new ObservableCollection<UserModel>(_allUsers);
            MessageBox.Show($"Loaded users: {Users[0].Username}");
            IsLoading = false;
        }

        public async Task AddUserAsync(UserModel user)
        {
            var result = await _userService.AddUserAsync(user);
            if (result != null)
            {
                Users.Add(result);
            }
        }

        public async Task EditUserAsync(UserModel user)
        {
            var result = await _userService.EditUserAsync(user.Id, user);
            if (result != null)
            {
                var existing = Users.FirstOrDefault(u => u.Id == user.Id);
                if (existing != null)
                {
                    int index = Users.IndexOf(existing);
                    Users[index] = result;
                }
            }
        }

        public async Task DeleteUserAsync(UserModel user)
        {
            if (await _userService.DeleteUserAsync(user.Id))
            {
                Users.Remove(user);
            }
        }

        public async Task SearchUsersAsync(string phrase)
        {
            IsLoading = true;
            Users.Clear();
            var users = await _userService.SearchUsersAsync(phrase);
            foreach (var u in users)
                Users.Add(u);
            IsLoading = false;
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
