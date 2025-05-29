using Bookstore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Net.Http.Json;
using System.Windows;


namespace Bookstore.Services
{
    public class UserService
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "http://localhost:5257/api";
        private List<UserModel> _cachedUsers;

        public UserService()
        {
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(
                new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<UserModel> AddUserAsync(UserModel user)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync($"{BaseUrl}/User/Add", user);
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<UserModel>();
                }
                else
                {
                    Console.WriteLine($"Failed to add user. Status code: {response.StatusCode}");
                    return null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding user: {ex.Message}");
                return null;
            }
        }

        public async Task<UserModel> EditUserAsync(Guid userId, UserModel user)
        {
            try
            {
                if (user == null || userId == Guid.Empty)
                {
                    throw new ArgumentException("Nieprawidłowe dane użytkownika lub ID.");
                }

                var response = await _httpClient.PutAsJsonAsync($"{BaseUrl}/User/Edit/{userId}", user);

                if (response.IsSuccessStatusCode)
                {
                    var responseData = await response.Content.ReadFromJsonAsync<EditUserResponse>();
                    return responseData?.User ?? throw new Exception("Nie udało się pobrać zaktualizowanego użytkownika.");
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Nie udało się edytować użytkownika. Status: {response.StatusCode}, Treść: {errorContent}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Błąd podczas edycji użytkownika: {ex.Message}");
                throw;
            }
        }

        public class EditUserResponse
        {
            public string Message { get; set; }
            public UserModel User { get; set; }
        }

        public async Task<bool> DeleteUserAsync(Guid userId)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"{BaseUrl}/User/Delete/{userId}");
                response.EnsureSuccessStatusCode();
                _cachedUsers = null; // Invalidate cache
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting user: {userId}: {ex.Message}");
                return false;
            }
        }

        public async Task<List<UserModel>> GetAllAsync()
        {
            try
            {
                var users = await _httpClient.GetFromJsonAsync<List<UserModel>>($"{BaseUrl}/User/GetAll");
                _cachedUsers = users;
                
                return users;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching users: {ex.Message}");
                return new List<UserModel>();
            }
        }

        public async Task<UserModel> GetByIdAsync(Guid userId)
        {
            try
            {
                var user = await _httpClient.GetFromJsonAsync<UserModel>($"{BaseUrl}/User/GetById/{userId}");
                return user ?? throw new Exception("Nie znaleziono użytkownika.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching user: {ex.Message}");
                throw;
            }
        }

        // Opcjonalnie: wyszukiwanie użytkowników przez API lub lokalnie w cache
        public async Task<List<UserModel>> SearchUsersAsync(string searchPhrase)
        {
            try
            {
                // Jeśli masz endpoint API do wyszukiwania:
                try
                {
                    return await _httpClient.GetFromJsonAsync<List<UserModel>>(
                        $"{BaseUrl}/User/Search?phrase={Uri.EscapeDataString(searchPhrase)}");
                }
                catch
                {
                    // Jeśli API nie jest dostępne, użyj lokalnego filtrowania
                    if (_cachedUsers == null || _cachedUsers.Count == 0)
                    {
                        _cachedUsers = await GetAllAsync();
                    }

                    if (string.IsNullOrWhiteSpace(searchPhrase))
                    {
                        return _cachedUsers;
                    }

                    searchPhrase = searchPhrase.ToLower();
                    return _cachedUsers.Where(user =>
                        (user.Email?.ToLower().Contains(searchPhrase) ?? false) ||
                        (user.Username?.ToLower().Contains(searchPhrase) ?? false) ||
                        (user.FirstName?.ToLower().Contains(searchPhrase) ?? false) ||
                        (user.LastName?.ToLower().Contains(searchPhrase) ?? false)
                    ).ToList();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error searching users: {ex.Message}");
                return new List<UserModel>();
            }
        }
    }
}
