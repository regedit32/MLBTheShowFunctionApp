using Microsoft.Azure.Cosmos;
using MLBTheShowSharp.Models.Interfaces;
using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace MLBTheShowSharp.Services
{
    internal class CosmosDbService : IDbService
    {
        // The Azure Cosmos DB endpoint.
        private static readonly string EndpointUri = Environment.GetEnvironmentVariable("EndPointUri");

        // The primary key for the Azure Cosmos account.
        private static readonly string PrimaryKey = Environment.GetEnvironmentVariable("PrimaryKey");

        private readonly ILogger _log;

        // The Cosmos client instance
        private readonly CosmosClient _cosmosClient;

        // The database we will create
        private Database _database;

        // The container we will create.
        private Container _container;

        public CosmosDbService(string databaseId, string containerId, ILogger logger)
        {
            _log = logger ?? throw new ArgumentNullException(nameof(logger));
            _cosmosClient = new CosmosClient(EndpointUri, PrimaryKey, new CosmosClientOptions() { ApplicationName = "mlbTheShowApp" });
            CreateDatabaseAsync(databaseId).Wait();
            CreateContainerAsync(containerId).Wait();
        }

        /// <summary>
        /// Create the database if it does not exist
        /// </summary>
        internal async Task CreateDatabaseAsync(string databaseId)
        {
            _database = await _cosmosClient.CreateDatabaseIfNotExistsAsync(databaseId);
        }

        /// <summary>
        /// Create the container if it does not exist.
        /// </summary>
        internal async Task CreateContainerAsync(string containerId)
        {
            _container = await _database.CreateContainerIfNotExistsAsync(containerId, "/partitionKey");
        }

        public async Task AddItemsAsync<T>(IEnumerable<T> items) where T : IItem, new()
        {
            foreach (var item in items)
            {
                await AddItemAsync(item);
            }
        }

        public async Task AddItemAsync<T>(T item) where T : IItem, new()
        {
            try
            {
                ItemResponse<T> response = await _container.ReadItemAsync<T>(item.Id, new PartitionKey(item.PartitionKey));
                _log.LogInformation($"Item in database '{_database}', with id: {response.Resource.Id} already exists.");
            }
            catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
            {
                await _container.CreateItemAsync(item, new PartitionKey(item.PartitionKey));
            }
            catch (CosmosException de)
            {
                Exception baseException = de.GetBaseException();
                _log.LogError("{0} error occurred: {1}", de.StatusCode, de);
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

        public void Dispose()
        {
            _cosmosClient.Dispose();
        }
    }
}