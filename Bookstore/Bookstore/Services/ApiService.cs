using Bookstore.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.IO;


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
        public async Task<Offer> AddOfferAsync(Offer offer)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync($"{BaseUrl}/Offers/Add", offer);

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<Offer>();
                }
                else
                {
                    Console.WriteLine($"Failed to add offer. Status code: {response.StatusCode}");
                    return null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding offer: {ex.Message}");
                return null;
            }
        }

        public async Task<List<Offer>> GetOffersAsync()
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<List<Offer>>($"{BaseUrl}/offers/GetAll");
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
                return await _httpClient.GetFromJsonAsync<List<Offer>>($"{BaseUrl}/offers/GetPublic");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching public offers: {ex.Message}");
                return new List<Offer>();
            }
        }

        public async Task<List<Offer>> GetByRequesterAsync(Guid requesterId)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<List<Offer>>($"{BaseUrl}/offers/GetByREquester/{requesterId}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching requester's offers: {ex.Message}");
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
        public async Task<FileModel> GetFileByIdAsync(int fileId)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<FileModel>($"{BaseUrl}/file/{fileId}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Błąd podczas pobierania pliku o ID {fileId}: {ex.Message}");
                return null;
            }
        }
        public async Task<FileModel> UploadFileAsync(string filePath)
        {
            try
            {
                using (var content = new MultipartFormDataContent())
                {
                    using (var fileStream = File.OpenRead(filePath))
                    {
                        content.Add(new StreamContent(fileStream), "file", Path.GetFileName(filePath));

                        var response = await _httpClient.PostAsync($"{BaseUrl}/file/upload", content);
                        if (response.IsSuccessStatusCode)
                        {
                            var result = await response.Content.ReadFromJsonAsync<FileUploadResponse>();
                            if (result != null && result.File != null)
                            {
                                return result.File;
                            }
                            Console.WriteLine("Nie udało się pobrać danych pliku z odpowiedzi serwera.");
                            return null;
                        }
                        Console.WriteLine($"Nie udało się wgrać pliku. Kod statusu: {response.StatusCode}");
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Błąd podczas wgrywania pliku: {ex.Message}");
                return null;
            }
        }
    }
}