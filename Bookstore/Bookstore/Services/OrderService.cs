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

        public async Task<List<OrderDetailModel>> GetDetailedByUserIdAsync(Guid userId)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<List<OrderDetailModel>>($"{BaseUrl}/Order/GetDetailedByUserId/{userId}") 
                       ?? new List<OrderDetailModel>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching user orders: {ex.Message}");
                return new List<OrderDetailModel>();
            }
        }

        public async Task<List<Order>> GetAllAsync()
        {
            try
            {
                var detailedOrders = await GetAllDetailedAsync();
                return ConvertToOrders(detailedOrders);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching orders: {ex.Message}");
                return new List<Order>();
            }
        }

        public async Task<List<Order>> GetByUserIdAsync(Guid userId)
        {
            try
            {
                var detailedOrders = await GetDetailedByUserIdAsync(userId);
                return ConvertToOrders(detailedOrders);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching user orders: {ex.Message}");
                return new List<Order>();
            }
        }

        private List<Order> ConvertToOrders(List<OrderDetailModel> detailedOrders)
        {
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

        // Konwertuje nazwę statusu na odpowiednią wartość Enum
        public int GetOrderStateEnumFromDisplayName(string statusDisplayName)
        {
            switch (statusDisplayName.Trim())
            {
                case "Nieopłacone": return 0;
                case "Opłacone": return 10;
                case "Wysłane": return 20;
                case "Zwrócone (wysłane)": return 25;
                case "Odebrane": return 30;
                case "Zwrócone (odebrane)": return 35;
                case "Anulowane": return 40;
                default: return 0;
            }
        }

        // Pobiera listę dostępnych statusów zamówień
        public List<string> GetAvailableOrderStatuses()
        {
            return new List<string>
            {
                "Nieopłacone",
                "Opłacone",
                "Wysłane",
                "Zwrócone (wysłane)",
                "Odebrane",
                "Zwrócone (odebrane)",
                "Anulowane"
            };
        }

        // Aktualizuje status zamówienia
        public async Task<bool> UpdateOrderStatusAsync(int orderId, string newStatus)
        {
            try
            {
                // Pobierz aktualne zamówienie
                var orderDetail = await _httpClient.GetFromJsonAsync<OrderDetailModel>($"{BaseUrl}/Order/GetById/{orderId}");
                if (orderDetail == null)
                    return false;

                // Przygotuj obiekt do aktualizacji
                var orderToUpdate = new
                {
                    Id = orderId,
                    BuyerId = orderDetail.BuyerId,
                    OrderStateEnum = GetOrderStateEnumFromDisplayName(newStatus)
                };

                // Wywołaj API do aktualizacji
                var response = await _httpClient.PutAsJsonAsync($"{BaseUrl}/Order/Edit/{orderId}", orderToUpdate);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating order status: {ex.Message}");
                return false;
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
