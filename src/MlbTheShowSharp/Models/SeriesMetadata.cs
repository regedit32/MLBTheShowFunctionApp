using MLBTheShowSharp.Models.Interfaces;
using Newtonsoft.Json;

namespace MLBTheShowSharp.Models
{
    internal class SeriesMetadata : IItem
    {
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "series_id")]
        public string Id { get; set; }

        public string PartitionKey { get { return Name; } }
    }
}
