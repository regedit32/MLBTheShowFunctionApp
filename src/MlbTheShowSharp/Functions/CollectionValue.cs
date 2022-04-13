using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using MLBTheShowSharp.Services;
using System.Net.Http;
using System.Threading.Tasks;

namespace MLBTheShowSharp.Functions
{
    public class CollectionValue
    {
        private readonly HttpClient _httpClient;

        public CollectionValue(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        [FunctionName("ProcessLiveSeriesValue")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("HTTP trigger function processed a request.");

            var httpClientService = new HttpClientService(log, _httpClient);
            var collectionService = new CollectionService(log, httpClientService);

            var result = await collectionService.ProcessLiveSeriesValue();
            return new OkObjectResult(result);
        }
    }
}