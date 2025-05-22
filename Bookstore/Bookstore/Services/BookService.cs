using Bookstore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;


namespace Bookstore.Services
{
    public class BookService
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "http://localhost:5257/api";
        private List<BookModel> _cachedBooks;

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

        public async Task<BookModel> EditBookAsync(int bookId, BookModel book)
        {
            try
            {
                if (book == null || bookId <= 0)
                {
                    throw new ArgumentException("Nieprawidłowe dane książki lub ID.");
                }

                var response = await _httpClient.PutAsJsonAsync($"{BaseUrl}/Book/Edit/{bookId}", book);

                if (response.IsSuccessStatusCode)
                {
                    var responseData = await response.Content.ReadFromJsonAsync<EditBookResponse>();
                    return responseData?.Book ?? throw new Exception("Nie udało się pobrać zaktualizowanej książki.");
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Nie udało się edytować książki. Status: {response.StatusCode}, Treść: {errorContent}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Błąd podczas edycji książki: {ex.Message}");
                throw;
            }
        }

        public class EditBookResponse
        {
            public string Message { get; set; }
            public BookModel Book { get; set; }
        }

        public async Task<bool> DeletedAsync(int bookId)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"{BaseUrl}/Book/Delete/{bookId}");
                response.EnsureSuccessStatusCode();
                // Invalidate cache
                _cachedBooks = null;
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
                var books = await _httpClient.GetFromJsonAsync<List<BookModel>>($"{BaseUrl}/Book/GetAll");
                _cachedBooks = books; // Cache the results
                return books;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching books: {ex.Message}");
                return new List<BookModel>();
            }
        }

        public async Task<BookModel> GetByIdAsync(int bookId)
        {
            try
            {
                var book = await _httpClient.GetFromJsonAsync<BookModel>($"{BaseUrl}/Book/GetById/{bookId}");
                return book ?? throw new Exception("Nie znaleziono książki.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching book: {ex.Message}");
                throw;
            }
        }

        // Metoda do lokalnego wyszukiwania książek
        public async Task<List<BookModel>> SearchBooksAsync(string searchPhrase)
        {
            try
            {
                // Próba użycia API (może być zaimplementowane w przyszłości)
                try
                {
                    return await _httpClient.GetFromJsonAsync<List<BookModel>>($"{BaseUrl}/Book/Search?phrase={Uri.EscapeDataString(searchPhrase)}");
                }
                catch
                {
                    // Jeśli API nie jest dostępne, użyj lokalnego filtrowania
                    if (_cachedBooks == null || _cachedBooks.Count == 0)
                    {
                        _cachedBooks = await GetAllAsync();
                    }

                    if (string.IsNullOrWhiteSpace(searchPhrase))
                    {
                        return _cachedBooks;
                    }

                    searchPhrase = searchPhrase.ToLower();
                    return _cachedBooks.Where(book =>
                        (book.Title?.ToLower().Contains(searchPhrase) ?? false) ||
                        (book.Description?.ToLower().Contains(searchPhrase) ?? false) ||
                        (book.Isbn?.ToLower().Contains(searchPhrase) ?? false) ||
                        (book.AuthorDisplay?.ToLower().Contains(searchPhrase) ?? false) ||
                        (book.GenreDisplay?.ToLower().Contains(searchPhrase) ?? false)
                    ).ToList();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error searching books: {ex.Message}");
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
                Console.WriteLine($"Error fetching authors: {ex.Message}");
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
                Console.WriteLine($"Error fetching series: {ex.Message}");
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
                Console.WriteLine($"Error fetching genres: {ex.Message}");
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
                Console.WriteLine($"Error fetching publishers: {ex.Message}");
                return new List<PublisherModel>();
            }
        }
    }
}