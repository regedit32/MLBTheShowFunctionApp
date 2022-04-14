using Newtonsoft.Json;

namespace MLBTheShowSharp.Models.Interfaces
{
    public interface IDbItem
    {
        [JsonProperty(PropertyName = "id")]
        string Id { get; set; }

        [JsonProperty(PropertyName = "partitionKey")]
        string PartitionKey { get; }
    }
}