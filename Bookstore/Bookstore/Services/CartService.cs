using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace Bookstore.Services
{
    public class CartService
    {
        private static readonly string CartFilePath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "Bookstore",
        "cart.json");

        public CartService()
        {
            EnsureCartFileExists();
        }

        private void EnsureCartFileExists()
        {
            string folder = Path.GetDirectoryName(CartFilePath);

            if (folder == null)
                throw new InvalidOperationException("Nie można określić folderu dla ścieżki koszyka.");

            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            if (!File.Exists(CartFilePath))
                File.WriteAllText(CartFilePath, "[]");
        }

        public static List<int> LoadCart()
        {
            if (!File.Exists(CartFilePath)) return new List<int>();

            var json = File.ReadAllText(CartFilePath);
            return JsonSerializer.Deserialize<List<int>>(json) ?? new List<int>();
        }

        public static void SaveCart(List<int> offerIds)
        {
            string folder = Path.GetDirectoryName(CartFilePath);
            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            var json = JsonSerializer.Serialize(offerIds);
            File.WriteAllText(CartFilePath, json);
        }


        public static void AddToCart(int offerId)
        {
            var cart = LoadCart();
            if (!cart.Contains(offerId))
            {
                cart.Add(offerId);
                SaveCart(cart);
            }
        }

        public static void RemoveFromCart(int offerId)
        {
            var cart = LoadCart();
            if (cart.Remove(offerId))
            {
                SaveCart(cart);
            }
        }
    }
}
