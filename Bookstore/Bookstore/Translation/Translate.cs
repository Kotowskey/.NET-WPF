using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Bookstore.Translation
{
    public static class Translate
    {
        public static string Get(string key)
        {
            return Application.Current.TryFindResource(key)?.ToString() ?? $"!{key}!";
        }
    }
}
