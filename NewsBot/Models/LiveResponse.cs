using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NewsBot.Models
{
    public class LiveResponse
    {
        public string Status { get; set; }
        public string Source { get; set; }
        public string SortBy { get; set; }
        public List<Article> Articles { get; set;}
    }
}