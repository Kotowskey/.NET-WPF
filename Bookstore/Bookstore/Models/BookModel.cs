using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bookstore.Models
{
    public class BookModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Isbn { get; set; }
        public string PublicationYear { get; set; }
        public PublisherModel Publisher { get; set; }
        public SeriesModel Series { get; set; }
        public List<GenreModel> Genre { get; set; }
        public List<AuthorModel> Author { get; set; }
        public string PublisherDisplay => Publisher.Name;
        public string PublisherCountryDisplay => Publisher.Country;
        public string PublisherWithCountry => $"{PublisherDisplay} ({PublisherCountryDisplay})";

        public string SeriesDisplay => Series.Name;
        public string SeriesDescriptionDisplay => Series.Description;
        public string GenreDisplay => Genre != null && Genre.Any()
        ? string.Join(", ", Genre.Select(a => a.Name))
        : "";
        public string AuthorDisplay => Author != null && Author.Any()
        ? string.Join(", ", Author.Select(a => a.Name))
        : "";
    }
}
