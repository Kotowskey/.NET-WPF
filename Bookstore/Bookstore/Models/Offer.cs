using Bookstore.Models.Enums;
using Bookstore.Views;
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
        public string StatusDisplay
        {
            get
            {
                switch (OfferStateEnum)
                {
                    case 0:
                        return "Wersja robocza";
                    case 10:
                        return "Publiczne";
                    case 20:
                        return "Prywatne";
                    case 30:
                        return "Zrealizowane";
                    default:
                        return "Nieznany";
                }
            }
        }
        public string CreatedDateDisplay => $"{CreatedDate:d}";
    }
}