using System.Collections.Generic;

namespace MLBTheShowSharp.Models
{
    public class ListingsResponse
    {
        public int page { get; set; }
        public int per_page { get; set; }
        public int total_pages { get; set; }
        public IList<Listing> listings { get; set; }
    }
}