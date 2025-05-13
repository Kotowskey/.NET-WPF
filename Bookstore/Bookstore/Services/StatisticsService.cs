using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Bookstore.Services
{
    public class StatisticsService
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "http://localhost:5257/api";

        public StatisticsService()
        {
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(
                new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<StatisticsModel> GetAllStatisticsAsync()
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<StatisticsModel>($"{BaseUrl}/Statistics/GetAll");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching statistics: {ex.Message}");
                return new StatisticsModel();
            }
        }
    }

    public class StatisticsModel
    {
        public int BooksCount { get; set; }
        public int AuthorsCount { get; set; }
        public int GenresCount { get; set; }
        public int PublishersCount { get; set; }
        public int UsersCount { get; set; }
    }
}
