using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Bookstore.Translation
{
    public static class LocalizationManager
    {
        public static void ChangeLanguage(string languageCode)
        {
            var dict = new ResourceDictionary();
            switch (languageCode)
            {
                case "en":
                    dict.Source = new Uri("Translation/StringResources.en.xaml", UriKind.Relative);
                    break;
                default:
                    dict.Source = new Uri("Translation/StringResources.pl.xaml", UriKind.Relative);
                    break;
            }

            var existingDict = Application.Current.Resources.MergedDictionaries
                                   .FirstOrDefault(d => d.Source?.OriginalString.StartsWith("Resources/StringResources.") == true);

            if (existingDict != null)
                Application.Current.Resources.MergedDictionaries.Remove(existingDict);

            Application.Current.Resources.MergedDictionaries.Add(dict);
        }

        public static string Get(string key)
        {
            return Application.Current.TryFindResource(key)?.ToString() ?? $"!{key}!";
        }
    }

}
