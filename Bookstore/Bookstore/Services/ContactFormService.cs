using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Windows;
using Bookstore.Models;

namespace Bookstore.Services
{
    public class ContactFormService
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "http://localhost:5257/api";

        public ContactFormService()
        {
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(
                new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<bool> SubmitContactFormAsync(ContactFormModel form)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync($"{BaseUrl}/ContactForm/submit", form);

                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
                else
                {
                    var content = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Error submitting form: {response.StatusCode}, {content}");
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception submitting contact form: {ex.Message}");
                return false;
            }
        }
    }
}
