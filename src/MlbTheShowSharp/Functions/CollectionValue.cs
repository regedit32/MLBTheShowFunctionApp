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
        public void Run([TimerTrigger("15 5,9,15,19,23 * * *")] TimerInfo myTimer, ILogger log, ExecutionContext executionContext)
        {
            log.LogInformation("HTTP trigger function processed a request.");

            var httpClientService = new HttpClientService(log, _httpClient);
            var collectionService = new CollectionService(log, httpClientService, executionContext);

            collectionService.ProcessLiveSeriesValue().Wait();
        }

        [FunctionName("GetLiveSeriesValue")]
        public async Task<IActionResult> Get(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log, ExecutionContext executionContext)
        {
            log.LogInformation("HTTP trigger function processed a request.");

            var httpClientService = new HttpClientService(log, _httpClient);
            var collectionService = new CollectionService(log, httpClientService, executionContext);

            var result = await collectionService.GetLiveSeriesValue();
            return new OkObjectResult(result);
        }
    }
}