using System;

namespace Bookstore.Models
{
    public class Offer
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public float Price { get; set; }
        public int BookId { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid RequesterId { get; set; }
        public int? FileId { get; set; }
        public int OfferStateEnum { get; set; }
        public string PriceDisplay => $"{Price:C}";
        public string CreatedDateDisplay => $"{CreatedDate:d}";
        public bool IsInCart { get; set; }
    }
}