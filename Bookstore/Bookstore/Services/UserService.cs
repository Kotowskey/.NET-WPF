using Bookstore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Windows;

namespace Bookstore.Services
{
    public class UserService
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "https://localhost:7109";
        private List<UserModel> _cachedUsers;

        public UserService(CookieContainer cookieContainer)
        {
            var handler = new HttpClientHandler
            {
                UseCookies = true,
                CookieContainer = cookieContainer
            };
            _httpClient = new HttpClient(handler)
            {
                BaseAddress = new Uri(BaseUrl)
            };
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(
                new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<UserModel> AddUserAsync(UserModel user)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/User/Add", user);
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<UserModel>();
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<UserModel> EditUserAsync(Guid userId, UserModel user)
        {
            try
            {
                if (user == null || userId == Guid.Empty)
                    throw new ArgumentException("Nieprawidłowe dane użytkownika lub ID.");

                var response = await _httpClient.PutAsJsonAsync($"api/User/Edit/{userId}", user);

                if (response.IsSuccessStatusCode)
                {
                    var responseData = await response.Content.ReadFromJsonAsync<EditUserResponse>();
                    return responseData?.User
                        ?? throw new Exception("UserService.EditUserAsync: nie udało się pobrać zaktualizowanego użytkownika.");
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    throw new Exception(
                        $"UserService.EditUserAsync failed: Status={response.StatusCode}, Treść={errorContent}");
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private class EditUserResponse
        {
            public string Message { get; set; }
            public UserModel User { get; set; }
        }

        public async Task<bool> DeleteUserAsync(Guid userId)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"api/User/Delete/{userId}");
                response.EnsureSuccessStatusCode();
                _cachedUsers = null;
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<List<UserModel>> GetAllAsync()
        {
            try
            {
                var users = await _httpClient.GetFromJsonAsync<List<UserModel>>("api/User/GetAll");
                _cachedUsers = users;
                return users;
            }
            catch (Exception ex)
            {
                return new List<UserModel>();
            }
        }

        public async Task<UserModel> GetByIdAsync(Guid userId)
        {
            try
            {
                var user = await _httpClient.GetFromJsonAsync<UserModel>($"api/User/GetById/{userId}");
                return user ?? throw new Exception("UserService.GetByIdAsync: Nie znaleziono użytkownika.");
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<List<UserModel>> SearchUsersAsync(string searchPhrase)
        {
            try
            {
                var response = await _httpClient.GetAsync($"Search?searchPhrase={WebUtility.UrlEncode(searchPhrase)}");
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<List<UserModel>>();
                }

                if (_cachedUsers == null || _cachedUsers.Count == 0)
                {
                    _cachedUsers = await GetAllAsync();
                }
                if (string.IsNullOrWhiteSpace(searchPhrase))
                {
                    return _cachedUsers;
                }
                var lowered = searchPhrase.ToLower();
                return _cachedUsers.Where(u =>
                    (u.Email?.ToLower().Contains(lowered) ?? false) ||
                    (u.Username?.ToLower().Contains(lowered) ?? false) ||
                    (u.FirstName?.ToLower().Contains(lowered) ?? false) ||
                    (u.LastName?.ToLower().Contains(lowered) ?? false)
                ).ToList();
            }
            catch (Exception ex)
            {
                return new List<UserModel>();
            }
        }
    }
}
