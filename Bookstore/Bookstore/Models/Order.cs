using System;

namespace Bookstore.Models
{
    public class Order
    {
        public int Id { get; set; }
        public Guid CustomerId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string BookTitle { get; set; } = string.Empty;
        public DateTime OrderDate { get; set; }
        public decimal Price { get; set; }
        public string Status { get; set; } = string.Empty;
        
        public string OrderDateDisplay => OrderDate.ToString("dd.MM.yyyy");
        public string PriceDisplay => $"{Price:C}";
    }
}
