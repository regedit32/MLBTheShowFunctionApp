using Newtonsoft.Json;

namespace MLBTheShowSharp.Models.Interfaces
{
    public interface IItem
    {
        [JsonProperty(PropertyName = "id")]
        string Id { get; set; }

        [JsonProperty(PropertyName = "partitionKey")]
        string PartitionKey { get; }
    }
}
