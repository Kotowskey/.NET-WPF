using System.Collections.Generic;

namespace Bookstore.Models
{
    public class StatisticsModel
    {
        public int TotalBooks { get; set; }
        public int TotalUsers { get; set; }
        public int ActiveUsers { get; set; }
        public Dictionary<string, int> TopGenres { get; set; } = new Dictionary<string, int>();
        public Dictionary<string, int> TopAuthors { get; set; } = new Dictionary<string, int>();
    }
}
