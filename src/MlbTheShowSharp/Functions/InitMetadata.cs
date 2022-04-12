using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using MLBTheShowSharp.Constants;
using MLBTheShowSharp.Services;
using System.Net.Http;
using System.Threading.Tasks;

namespace MLBTheShowSharp
{
    public class InitMetadata
    {
        private readonly HttpClient httpClient;

        public InitMetadata(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        [FunctionName("InitMetadata")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            MetadataService metadataService = new(log, httpClient);
            metadataService.InitializeSeriesDatabaseAsync(ContainerNames.SeriesMetadata).Wait();
            metadataService.InitializeLeagueDatabaseAsync(ContainerNames.LeagueMetadata);

            return new OkObjectResult("ok");
        }
    }
}