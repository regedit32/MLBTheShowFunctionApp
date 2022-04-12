using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using MLBTheShowSharp.Constants;
using MLBTheShowSharp.Models;
using MLBTheShowSharp.Models.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace MLBTheShowSharp.Services
{
    internal class MetadataService
    {
        private static readonly string _databaseName = Environment.GetEnvironmentVariable(SettingNames.DatabaseName);
        private readonly ILogger _log;
        private readonly HttpClient _httpClient;

        public MetadataService(ILogger logger, HttpClient httpClient)
        {
            _log = logger ?? throw new ArgumentNullException(nameof(logger));
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        public async Task InitializeSeriesDatabaseAsync(string containerName)
        {
            var uri = new Uri(new Uri(TheShowApi.BaseURI), TheShowApi.MetadataEndpoint);
            var response = await _httpClient.GetAsync(uri);
            var metadata = await response.Content.ReadAsAsync<TheShowMetadata>();

            InitializeDatabaseAsync(containerName, metadata.series).Wait();
        }

        public void InitializeLeagueDatabaseAsync(string containerName)
        {
            var leagueMetadata = GetLeagueMetadata();
            InitializeDatabaseAsync(containerName, leagueMetadata).Wait();
        }

        public async Task InitializeDatabaseAsync<T>(string containerName, IList<T> items) where T : IItem, new()
        {
            try
            {
                _log.LogInformation("Beginning operations to initialize Metadata");

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
                _log.LogError("Error: {0}", e);
            }
            finally
            {
                _log.LogInformation("Work Complete.");
            }
        }

        public static List<LeagueMetadata> GetLeagueMetadata()
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