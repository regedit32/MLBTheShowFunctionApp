using MLBTheShowSharp.Models.Interfaces;
using Newtonsoft.Json;

namespace MLBTheShowSharp.Models
{
    internal class Series : IItem
    {
        private string _id;

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "series_id")]
        public string SeriesId { get; set; }

        [JsonProperty(PropertyName = "id")]
        public string Id
        {
            get { return SeriesId; }
            set => _id = value;
        }

        public string PartitionKey
        { get { return Name; } }
    }
}