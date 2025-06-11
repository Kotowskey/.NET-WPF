using Bookstore.Models;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Bookstore.Services
{
    public class AuthService
    {
        private readonly HttpClient _httpClient;
        private readonly CookieContainer _cookieContainer;

        public AuthService()
        {
            _cookieContainer = new CookieContainer();
            var handler = new HttpClientHandler
            {
                UseCookies = true,
                CookieContainer = _cookieContainer
            };

            _httpClient = new HttpClient(handler)
            {
                BaseAddress = new Uri("https://localhost:7109")
            };
        }
        public CookieContainer CookieContainer => _cookieContainer;
        public async Task<bool> LoginAsync(string username, string password)
        {
            var request = new
            {
                Username = username,
                Password = password
            };
            var response = await _httpClient.PostAsJsonAsync("api/Auth/Login", request);
            return response.IsSuccessStatusCode;
        }
        public async Task<bool> RegisterAsync(string email, string username, string firstName, string lastName, string password)
        {
            var request = new
            {
                Email = email,
                Username = username,
                FirstName = firstName,
                LastName = lastName,
                Password = password
            };
            var response = await _httpClient.PostAsJsonAsync("api/Auth/Register", request);
            return response.IsSuccessStatusCode;
        }
        public async Task LogoutAsync()
        {
            await _httpClient.PostAsync("api/Auth/Logout", null);
            ClearAllCookies();
        }

        public async Task<bool> IsAdminAsync()
        {
            var response = await _httpClient.GetAsync("api/Auth/IsAdmin");
            if (!response.IsSuccessStatusCode) return false;
            var result = await response.Content.ReadFromJsonAsync<IsAdminResponse>();
            return result.isAdmin;
        }

        public async Task<Guid> GetUserIdAsync()
        {
            var response = await _httpClient.GetAsync("api/Auth/GetUserId");
            if (!response.IsSuccessStatusCode) return Guid.Empty;
            var result = await response.Content.ReadFromJsonAsync<GetUserIdResponse>();
            return result?.userId ?? Guid.Empty;
        }

        public async Task<StatisticsModel> GetStatisticsAsync()
        {
            var response = await _httpClient.GetAsync("api/Auth/GetStatistics");
            if (!response.IsSuccessStatusCode) return null;
            return await response.Content.ReadFromJsonAsync<StatisticsModel>();
        }

        private class IsAdminResponse
        {
            public bool isAdmin { get; set; }
        }

        private class GetUserIdResponse
        {
            public Guid userId { get; set; }
        }
        private void ClearAllCookies()
        {
            var table = (System.Collections.Hashtable)typeof(CookieContainer)
                .GetField("m_domainTable", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .GetValue(_cookieContainer);

            var domains = table.Keys.Cast<string>().ToList();

            foreach (var domain in domains)
            {
                var cookiesCollection = (CookieCollection)table[domain].GetType()
                    .GetField("m_list", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                    .GetValue(table[domain]);

                var cookies = cookiesCollection.Cast<Cookie>().ToList();
                foreach (var cookie in cookies)
                {
                    cookie.Expired = true;
                }
            }
        }
    }
}
