using Microsoft.Extensions.Logging;
using MLBTheShowSharp.Constants;
using MLBTheShowSharp.Models;
using MLBTheShowSharp.Utils;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;

namespace MLBTheShowSharp.Services
{
    internal class CollectionService
    {
        private readonly ILogger _log;
        private readonly HttpClientService _httpClientService;
        private static readonly string _databaseName = Environment.GetEnvironmentVariable(SettingNames.DatabaseName);

        public CollectionService(ILogger logger, HttpClientService httpClientService)
        {
            _log = logger ?? throw new ArgumentNullException(nameof(logger));
            _httpClientService = httpClientService ?? throw new ArgumentNullException(nameof(httpClientService));
        }

        public async Task ProcessLiveSeriesValue()
        {
            var values = await GetLiveSeriesValue();
            WriteCollectionValueItemsAsync(_databaseName, ContainerNames.LiveSeriesCollection, values).Wait();
        }

        public async Task WriteCollectionValueItemsAsync(string databaseName, string containerName, IEnumerable<CollectionValue> items)
        {
            CosmosDbService db = new(databaseName, containerName, _log);
            await db.AddItemsAsync(items);
        }

        public async Task<List<CollectionValue>> GetLiveSeriesValue()
        {
            List<CollectionValue> allValues = new();
            var teamValues = await GetLiveSeriesValueByTeam();
            var divisionValues = GetLiveSeriesValueByDivision(teamValues);
            var leagueValues = GetLiveSeriesValueByLeague(divisionValues);

            var total = new CollectionValue()
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Live Series",
                Buy = leagueValues.Sum(items => int.Parse(items.Buy)).ToString(),
                Sell = leagueValues.Sum(items => int.Parse(items.Sell)).ToString(),
            };

            allValues.Add(total);
            allValues.AddRange(divisionValues);
            allValues.AddRange(teamValues);
            allValues.AddRange(leagueValues);

            return allValues;
        }

        public async Task<List<CollectionValue>> GetLiveSeriesValueByTeam()
        {
            var teamData = MetadataService.GetLeagueMetadata();
            var collectionResult = new List<CollectionValue>();
            foreach (var team in teamData)
            {
                var query = new Dictionary<string, string>() {
                    { "series_id", "1337" },
                    { "team", team.TeamShortName }
                };
                var result = await GetCollectionAsync(query);

                collectionResult.Add(CreateLiveSeriesValue(team, result));
            }
            return collectionResult;
        }

        public static List<CollectionValue> GetLiveSeriesValueByDivision(List<CollectionValue> values)
        {
            List<CollectionValue> divisionResult = new();
            var divisionGroups = values.GroupBy(x => x.Division);

            foreach (var group in divisionGroups)
            {
                var value = new CollectionValue()
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = group.Select(x => x.Division).First(),
                    Division = group.Select(x => x.Division).First(),
                    League = group.Select(x => x.League).First(),
                    Buy = group.Sum(items => int.Parse(items.Buy)).ToString(),
                    Sell = group.Sum(items => int.Parse(items.Sell)).ToString(),
                };

                divisionResult.Add(value);
            }

            return divisionResult;
        }

        public static List<CollectionValue> GetLiveSeriesValueByLeague(List<CollectionValue> values)
        {
            List<CollectionValue> divisionResult = new();
            var divisionGroups = values.GroupBy(x => x.League);

            foreach (var group in divisionGroups)
            {
                var value = new CollectionValue()
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = group.Select(x => x.League).First(),
                    Division = group.Select(x => x.League).First(),
                    League = group.Select(x => x.League).First(),
                    Buy = group.Sum(items => int.Parse(items.Buy)).ToString(),
                    Sell = group.Sum(items => int.Parse(items.Sell)).ToString(),
                };

                divisionResult.Add(value);
            }

            return divisionResult;
        }

        public static CollectionValue CreateLiveSeriesValue(LeagueMetadata teamData, List<Listing> result)
        {
            var value = new CollectionValue()
            {
                Id = Guid.NewGuid().ToString(),
                Name = teamData.TeamShortName,
                Division = teamData.Division,
                League = teamData.League,
                Buy = result.Sum(items => items.best_buy_price).ToString(),
                Sell = result.Sum(items => items.best_sell_price).ToString(),
            };

            return value;
        }

        public async Task<List<Listing>> GetCollectionAsync(Dictionary<string, string> parameters)
        {
            var uri = UriHelper.GetTheShowUri(TheShowApi.BaseURI, TheShowApi.ListingsEndpoint, parameters);

            var listingsResponse = await _httpClientService.GetResponseAsync<ListingsResponse>(uri);

            List<Listing> collection = new();
            collection.AddRange(listingsResponse.listings);

            if (listingsResponse.total_pages > 1)
            {
                var moreResults = await GetPaginatedResultsAsync(uri, listingsResponse.total_pages);
                collection.AddRange(moreResults);
            }

            return collection;
        }

        private async Task<List<Listing>> GetPaginatedResultsAsync(Uri uri, int total_pages)
        {
            List<Listing> collection = new();
            for (int i = 2; i <= total_pages; i++)
            {
                var newUri = uri.AddOrUpdateQuery(new NameValueCollection() { { "page", i.ToString() } });

                var listingsResponse = await _httpClientService.GetResponseAsync<ListingsResponse>(newUri);

                collection.AddRange(listingsResponse.listings);
            }

            return collection;
        }
    }
}