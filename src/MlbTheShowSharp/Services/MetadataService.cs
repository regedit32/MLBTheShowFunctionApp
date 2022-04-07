using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using MLBTheShowSharp.Constants;
using MLBTheShowSharp.Models;
using MLBTheShowSharp.Models.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace MLBTheShowSharp.Services
{
    internal class MetadataService
    {
        private static readonly string _databaseName = Environment.GetEnvironmentVariable(SettingNames.DatabaseName);
        private readonly ILogger log;

        public MetadataService(ILogger logger)
        {
            log = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public void InitializeLeagueDatabaseAsync(string containerName)
        {
            var leagueMetadata = GetLeagueMetadata();
            InitializeDatabaseAsync(containerName, leagueMetadata).Wait();
        }

        public async Task InitializeDatabaseAsync<T>(string containerName, List<T> items) where T : IItem, new()
        {
            try
            {
                log.LogInformation("Beginning operations to initialize Metadata");

                CosmosDbService db = new(_databaseName, containerName);

                foreach (var item in items)
                {
                    await db.AddItemAsync(item);
                }
                db.Dispose();

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

        }

        public List<LeagueMetadata> GetLeagueMetadata()
        {
            return ReadJson<List<LeagueMetadata>>(@"Resources\importLeagueMetadata.json");
        }

        public static T ReadJson<T>(string filePath)
        {
            string text = File.ReadAllText(filePath);
            return JsonConvert.DeserializeObject<T>(text);
        }
    }
}
