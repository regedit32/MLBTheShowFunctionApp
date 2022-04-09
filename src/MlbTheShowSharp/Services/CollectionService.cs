using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using MLBTheShowSharp.Constants;
using MLBTheShowSharp.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace MLBTheShowSharp.Services
{
    internal class CollectionService
    {
        private readonly ILogger _log;
        private readonly HttpClient _httpClient;

        public CollectionService(ILogger logger, HttpClient httpClient)
        {
            _log = logger ?? throw new ArgumentNullException(nameof(logger));
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        public async Task GetCollectionAsync(Dictionary<string, string> parameters)
        {
            // 'https://mlb22.theshow.com/apis/listings.json?series_id=1337&team=nym'
            var uri = GetTheShowUri(TheShowApi.BaseURI, TheShowApi.ListingsEndpoint, parameters);

            var response = await _httpClient.GetAsync(uri);
            var listingsResponse = await response.Content.ReadAsAsync<ListingsResponse>();

            List<Listing> collection = new();
            collection.AddRange(listingsResponse.listings);

            if (listingsResponse.total_pages > 1)
            {
                for (int i = 2; i <= listingsResponse.total_pages; i++)
                {

                }
            }
        }

        public static Uri GetTheShowUri(string baseUri, string endpoint, Dictionary<string, string> parameters)
        {
            var url = string.Format("{0}{1}", baseUri, endpoint);
            var newUri = new Uri(QueryHelpers.AddQueryString(url, parameters));

            return newUri;
        }
    }
}
