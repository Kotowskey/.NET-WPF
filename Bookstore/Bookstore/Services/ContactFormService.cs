using System;
using System.Collections.Generic;
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
        public async Task<List<ContactFormModel>> GetAllMessagesAsync()
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<List<ContactFormModel>>($"{BaseUrl}/ContactForm/messages");
                var mess = await HasActiveMessagesAsync();
                return response;
            }
            catch
            {
                return new List<ContactFormModel>();
            }
        }

        public async Task<ContactFormModel> EditMessageAsync(int id, ContactFormModel message)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"{BaseUrl}/ContactForm/archive/{id}", message);
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<ContactFormModel>();
                }
                return null;
            }
            catch
            {
                return null;
            }
        }
        public async Task<bool> HasActiveMessagesAsync()
        {
            try
            {
                var result = await _httpClient.GetFromJsonAsync<bool>($"{BaseUrl}/ContactForm/hasactive");
                if (result == null)
                {
                    return false; // lub obsłuż błąd wg potrzeb
                }
                if (result == true)
                {
                    MessageBox.Show("Masz aktywne wiadomości w formularzu kontaktowym.", "Informacja", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                    return result;
            }
            catch (Exception)
            {
                // obsłuż błąd wg potrzeb, np. loguj
                return false;
            }
        }
    }
}
