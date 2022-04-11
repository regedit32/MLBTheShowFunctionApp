using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using MLBTheShowSharp.Services;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace MLBTheShowSharp.Functions
{
    public class GetCollectionValue
    {
        private readonly HttpClient _httpClient;

        public GetCollectionValue(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        [FunctionName("GetCollectionValue")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            var collectionService = new CollectionService(log, _httpClient);
            // 'https://mlb22.theshow.com/apis/listings.json?series_id=1337&team=nym'

            var query = new Dictionary<string, string>() {
                { "series_id", "1337" },
                { "team", "nym" }
            };
            var result = await collectionService.GetCollectionAsync(query);
            return new OkObjectResult(result);
        }
    }
}
