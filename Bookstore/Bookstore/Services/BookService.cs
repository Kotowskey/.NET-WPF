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
        private const string BaseUrl = "http://localhost:5257/api";


        public BookService()
        {
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(
                new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<BookModel> AddBookAsync(BookModel book)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync($"{BaseUrl}/Book/Add", book);

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<BookModel>();
                }
                else
                {
                    Console.WriteLine($"Failed to add book. Status code: {response.StatusCode}");
                    return null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding book: {ex.Message}");
                return null;
            }
        }

        public async Task<bool> DeletedAsync(int bookId)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"{BaseUrl}/Book/Delete?bookId={bookId}");
                response.EnsureSuccessStatusCode();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error delete book: {bookId}: {ex.Message}");
                return false;
            }
        }

        public async Task<List<BookModel>> GetAllAsync()
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<List<BookModel>>($"{BaseUrl}/Book/GetAll");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching books: {ex.Message}");
                return new List<BookModel>();
            }
        }

        public async Task<List<AuthorModel>> GetAllAuthorAsync()
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<List<AuthorModel>>($"{BaseUrl}/Author/GetAll");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching books: {ex.Message}");
                return new List<AuthorModel>();
            }
        }

        public async Task<List<SeriesModel>> GetAllSeriesAsync()
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<List<SeriesModel>>($"{BaseUrl}/Series/GetAll");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching books: {ex.Message}");
                return new List<SeriesModel>();
            }
        }

        public async Task<List<GenreModel>> GetAllGenreAsync()
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<List<GenreModel>>($"{BaseUrl}/Genre/GetAll");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching books: {ex.Message}");
                return new List<GenreModel>();
            }
        }

        public async Task<List<PublisherModel>> GetAllPublisherAsync()
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<List<PublisherModel>>($"{BaseUrl}/Publisher/GetAll");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching books: {ex.Message}");
                return new List<PublisherModel>();
            }
        }
    }
}