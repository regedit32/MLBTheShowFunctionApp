using MLBTheShowSharp.Models.Interfaces;
using System;

namespace MLBTheShowSharp.Models
{
    internal class CollectionValue : IItem
    {
        public string Name { get; set; }

        public string Id { get; set; }

        public string PartitionKey
        { get { return Name; } }

        public string Sell { get; set; }

        public string Buy { get; set; }

        public string Division { get; set; }

        public string League { get; set; }

        public DateTime DateTime { get; set; } = DateTime.UtcNow;
    }
}