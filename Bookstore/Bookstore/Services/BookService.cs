using Bookstore.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;


namespace Bookstore.Services
{
    public class BookService
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "http://localhost:5257/api/Book";


        public BookService()
        {
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(
                new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<BookModel> GetOfferByIdAsync(BookModel book)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<BookModel>($"{BaseUrl}/Add/{book}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding book: {book}: {ex.Message}");
                return null;
            }
        }

        public async Task<List<BookModel>> GetAllAsync()
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<List<BookModel>>($"{BaseUrl}/GetAll");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching books: {ex.Message}");
                return new List<BookModel>();
            }
        }
    }
}