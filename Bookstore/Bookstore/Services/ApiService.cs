using Bookstore.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;


namespace Bookstore.Services
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "http://localhost:5257/api"; 


        public ApiService()
        {
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(
                new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<List<Offer>> GetOffersAsync()
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<List<Offer>>($"{BaseUrl}/offers");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching offers: {ex.Message}");
                return new List<Offer>();
            }
        }

        public async Task<List<Offer>> GetPublicOffersAsync()
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<List<Offer>>($"{BaseUrl}/offers/public");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching public offers: {ex.Message}");
                return new List<Offer>();
            }
        }

        public async Task<Offer> GetOfferByIdAsync(int id)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<Offer>($"{BaseUrl}/offers/{id}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching offer {id}: {ex.Message}");
                return null;
            }
        }
    }
}