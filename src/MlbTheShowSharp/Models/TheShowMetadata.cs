using System.Collections.Generic;

namespace MLBTheShowSharp.Models
{
    internal class TheShowMetadata
    {
        public IList<Series> series { get; set; }
        public IList<Brand> brands { get; set; }
    }
}