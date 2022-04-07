using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MLBTheShowSharp.Models
{
    internal class TheShowMetadata
    {
        public IList<Series> series { get; set; }
        public IList<Brand> brands { get; set; }
    }
}
