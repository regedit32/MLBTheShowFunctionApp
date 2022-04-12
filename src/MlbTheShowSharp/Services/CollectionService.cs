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

        public CollectionService(ILogger logger, HttpClientService httpClientService)
        {
            _log = logger ?? throw new ArgumentNullException(nameof(logger));
            _httpClientService = httpClientService ?? throw new ArgumentNullException(nameof(httpClientService));
        }

        public async Task<List<CollectionValue>> ProcessLiveSeriesValue()
        {
            var teamData = MetadataService.GetLeagueMetadata();
            var collectionResult = new List<CollectionValue>();
            foreach (var team in teamData)
            {
                // sample 'https://mlb22.theshow.com/apis/listings.json?series_id=1337&team=nym'
                var query = new Dictionary<string, string>() {
                    { "series_id", "1337" },
                    { "team", team.TeamShortName }
                };
                var result = await GetCollectionAsync(query);

                collectionResult.Add(CreateLiveSeriesValue(team, result));
            }
            return collectionResult;
        }

        public static CollectionValue CreateLiveSeriesValue(LeagueMetadata teamData, List<Listing> result)
        {
            var value = new CollectionValue()
            {
                 Name = teamData.TeamShortName,
                 Division = teamData.Division,
                 League = teamData.League,
                 Buy = result.Sum(items => Convert.ToInt32(items.best_buy_price)).ToString(),
                 Sell = result.Sum(items => Convert.ToInt32(items.best_sell_price)).ToString(),
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
