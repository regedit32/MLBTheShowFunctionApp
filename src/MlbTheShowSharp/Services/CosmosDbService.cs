using Microsoft.Azure.Cosmos;
using MLBTheShowSharp.Models.Interfaces;
using System;
using System.Net;
using System.Threading.Tasks;

namespace MLBTheShowSharp.Services
{
    internal class CosmosDbService : IDbService
    {
        // The Azure Cosmos DB endpoint for running this sample.
        private static readonly string EndpointUri = Environment.GetEnvironmentVariable("EndPointUri");

        // The primary key for the Azure Cosmos account.
        private static readonly string PrimaryKey = Environment.GetEnvironmentVariable("PrimaryKey");

        // The Cosmos client instance
        private readonly CosmosClient cosmosClient;

        // The database we will create
        private Database database;

        // The container we will create.
        private Container container;

        public CosmosDbService(string databaseId, string containerId)
        {
            cosmosClient = new CosmosClient(EndpointUri, PrimaryKey, new CosmosClientOptions() { ApplicationName = "mlbTheShowApp" });
            CreateDatabaseAsync(databaseId).Wait();
            CreateContainerAsync(containerId).Wait();
        }

        /// <summary>
        /// Create the database if it does not exist
        /// </summary>
        internal async Task CreateDatabaseAsync(string databaseId)
        {
            database = await this.cosmosClient.CreateDatabaseIfNotExistsAsync(databaseId);
            Console.WriteLine("Created Database: {0}\n", this.database.Id);
        }

        /// <summary>
        /// Create the container if it does not exist. 
        /// </summary>
        internal async Task CreateContainerAsync(string containerId)
        {
            this.container = await this.database.CreateContainerIfNotExistsAsync(containerId, "/partitionKey");
            Console.WriteLine("Created Container: {0}\n", this.container.Id);
        }

        public async Task AddItemAsync<T>(T item) where T : IItem, new()
        {
            try
            {
                ItemResponse<T> response = await container.ReadItemAsync<T>(item.Id, new PartitionKey(item.PartitionKey));
                Console.WriteLine("Item in database with id: {0} already exists\n", response.Resource.Id);
            }
            catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
            {
                await container.CreateItemAsync(item, new PartitionKey(item.PartitionKey));
            }
        }

        public void Dispose()
        {
            cosmosClient.Dispose();
        }
    }
}
