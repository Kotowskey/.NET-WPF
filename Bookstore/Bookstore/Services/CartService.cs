using Bookstore.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace Bookstore.Services
{
    public class CartService
    {
        private readonly string _cartFilePath;

        public CartService(Guid userId)
        {
            var baseFolder = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "Bookstore",
                userId.ToString());
            if (!Directory.Exists(baseFolder))
                Directory.CreateDirectory(baseFolder);

            _cartFilePath = Path.Combine(baseFolder, "cart.json");
            if (!File.Exists(_cartFilePath))
                File.WriteAllText(_cartFilePath, "[]");
        }

        public List<Offer> LoadCart()
        {
            var json = File.ReadAllText(_cartFilePath);
            return JsonSerializer.Deserialize<List<Offer>>(json);
        }

        public void SaveCart(List<Offer> offers)
        {
            var json = JsonSerializer.Serialize(offers);
            File.WriteAllText(_cartFilePath, json);
        }

        public void Add(Offer offer)
        {
            var cart = LoadCart();
            if (!cart.Contains(offer))
            {
                cart.Add(offer);
                SaveCart(cart);
            }
        }

        public void Remove(Offer offer)
        {
            var cart = LoadCart();
            var toRemove = cart.FirstOrDefault(o => o.Id == offer.Id);
            if (toRemove != null)
            {
                cart.Remove(toRemove);
                SaveCart(cart);
            }
        }
    }

}
