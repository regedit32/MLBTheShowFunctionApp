using Microsoft.Azure.WebJobs;
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
        private readonly HttpClientService _httpClientService;
        private readonly string _functionDirectory;

        public MetadataService(ILogger logger, HttpClientService httpClientService, ExecutionContext executionContext)
        {
            _log = logger ?? throw new ArgumentNullException(nameof(logger));
            _httpClientService = httpClientService ?? throw new ArgumentNullException(nameof(httpClientService));
            _functionDirectory = Directory.GetParent(executionContext.FunctionDirectory).FullName;
        }

        public async Task InitializeSeriesDatabaseAsync(string containerName)
        {
            var uri = new Uri(new Uri(TheShowApi.BaseURI), TheShowApi.MetadataEndpoint);
            var metadata = await _httpClientService.GetResponseAsync<TheShowMetadata>(uri);

            InitializeDatabaseAsync(containerName, metadata.series).Wait();
        }

        public void InitializeLeagueDatabaseAsync(string containerName)
        {
            var leagueMetadata = GetLeagueMetadata();
            InitializeDatabaseAsync(containerName, leagueMetadata).Wait();
        }

        public async Task InitializeDatabaseAsync<T>(string containerName, IList<T> items) where T : IDbItem, new()
        {
            _log.LogInformation("Beginning operations to initialize Metadata");

            CosmosDbService db = new(_databaseName, containerName, _log);
            await db.AddItemsAsync(items);
        }

        public List<LeagueMetadata> GetLeagueMetadata()
        {
            return ReadJson<List<LeagueMetadata>>($"{_functionDirectory}\\Resources\\importLeagueMetadata.json");
        }

        public static T ReadJson<T>(string filePath)
        {
            string text = File.ReadAllText(filePath);
            return JsonConvert.DeserializeObject<T>(text);
        }
    }
}