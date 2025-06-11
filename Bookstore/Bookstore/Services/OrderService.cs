using Bookstore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Bookstore.Services
{
    public class OrderService
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "https://localhost:7109/api";

        public OrderService()
        {
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(
                new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<List<OrderDetailModel>> GetAllDetailedAsync()
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<List<OrderDetailModel>>($"{BaseUrl}/Order/GetAllDetailed") 
                       ?? new List<OrderDetailModel>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching detailed orders: {ex.Message}");
                return new List<OrderDetailModel>();
            }
        }

        public async Task<List<Order>> GetAllAsync()
        {
            try
            {
                var detailedOrders = await GetAllDetailedAsync();
                var orders = new List<Order>();

                foreach (var detailedOrder in detailedOrders)
                {
                    var totalPrice = 0m;
                    var bookTitles = new List<string>();

                    foreach (var item in detailedOrder.OrderItems)
                    {
                        totalPrice += (decimal)item.Price;
                        if (!string.IsNullOrEmpty(item.OfferName))
                        {
                            bookTitles.Add(item.OfferName);
                        }
                    }

                    orders.Add(new Order
                    {
                        Id = detailedOrder.Id,
                        CustomerId = detailedOrder.BuyerId,
                        CustomerName = detailedOrder.CustomerName,
                        BookTitle = string.Join(", ", bookTitles),
                        OrderDate = DateTime.Now,
                        Price = totalPrice,
                        Status = GetStatusDisplayName(detailedOrder.OrderStateEnum)
                    });
                }

                return orders.OrderByDescending(o => o.OrderDate).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching orders: {ex.Message}");
                return new List<Order>();
            }
        }

        public async Task<List<Order>> SearchOrdersAsync(string searchPhrase)
        {
            try
            {
                var allOrders = await GetAllAsync();
                
                if (string.IsNullOrWhiteSpace(searchPhrase))
                    return allOrders;

                searchPhrase = searchPhrase.ToLower();
                return allOrders.Where(o =>
                    (o.CustomerName?.ToLower().Contains(searchPhrase) ?? false) ||
                    (o.BookTitle?.ToLower().Contains(searchPhrase) ?? false) ||
                    (o.Status?.ToLower().Contains(searchPhrase) ?? false) ||
                    o.Id.ToString().Contains(searchPhrase)
                ).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error searching orders: {ex.Message}");
                return new List<Order>();
            }
        }

        private string GetStatusDisplayName(int orderStateEnum)
        {
            switch (orderStateEnum)
            {
                case 0:
                    return "Nieopłacone";
                case 10:
                    return "Opłacone";
                case 20:
                    return "Wysłane";
                case 25:
                    return "Zwrócone (wysłane)";
                case 30:
                    return "Odebrane";
                case 35:
                    return "Zwrócone (odebrane)";
                case 40:
                    return "Anulowane";
                default:
                    return "Nieznany";
            }
        }
    }

    public class OrderDetailModel
    {
        public int Id { get; set; }
        public Guid BuyerId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public int OrderStateEnum { get; set; }
        public List<OrderItemDetailModel> OrderItems { get; set; } = new List<OrderItemDetailModel>();
    }

    public class OrderItemDetailModel
    {
        public int OfferId { get; set; }
        public string OfferName { get; set; } = string.Empty;
        public float Price { get; set; }
    }
}
