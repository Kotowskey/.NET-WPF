using System;

namespace Bookstore.Models
{
    public class Order
    {
        public int Id { get; set; }
        public string CustomerName { get; set; }
        public string BookTitle { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal Price { get; set; }
        public string Status { get; set; }
        public Guid CustomerId { get; set; }

        public string PriceDisplay => $"{Price:C}";
        public string OrderDateDisplay => $"{OrderDate:d}";
    }
}
