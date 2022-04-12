using MLBTheShowSharp.Models.Interfaces;
using Newtonsoft.Json;

namespace MLBTheShowSharp.Models
{
    public class LeagueMetadata : IItem
    {
        public string Team { get; set; }

        public string TeamShortName { get; set; }

        public string Division { get; set; }

        public string League { get; set; }

        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        public string PartitionKey
        { get { return TeamShortName; } }
    }
}