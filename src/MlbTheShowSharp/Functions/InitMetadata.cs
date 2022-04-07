using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using MLBTheShowSharp.Constants;
using MLBTheShowSharp.Services;
using System;
using System.Threading.Tasks;

namespace MLBTheShowSharp
{
    public static class InitMetadata
    {
        private static readonly string _databaseName = Environment.GetEnvironmentVariable(SettingNames.DatabaseName);

        [FunctionName("InitMetadata")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            try
            {
                log.LogInformation("Beginning operations to initialize Metadata");

                CosmosDbService leagueDb = new (_databaseName, ContainerNames.LeagueMetadata);

                MetadataService metadataService = new MetadataService();
                var leagueMetadata = metadataService.GetLeagueMetadata();
                foreach (var item in leagueMetadata)
                {
                    await leagueDb.AddItemAsync(item);
                }
                leagueDb.Dispose();

            }
            catch (CosmosException de)
            {
                Exception baseException = de.GetBaseException();
                Console.WriteLine("{0} error occurred: {1}", de.StatusCode, de);
            }
            catch (Exception e)
            {
                log.LogError("Error: {0}", e);
            }
            finally
            {
                log.LogInformation("Work Complete.");
            }

            return new OkObjectResult("ok");
        }
    }
}
