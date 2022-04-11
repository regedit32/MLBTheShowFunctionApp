using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using MLBTheShowSharp.Constants;
using MLBTheShowSharp.Models;
using MLBTheShowSharp.Utils;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
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

        public async Task<List<Listing>> GetCollectionAsync(Dictionary<string, string> parameters)
        {
            var uri = GetTheShowUri(TheShowApi.BaseURI, TheShowApi.ListingsEndpoint, parameters);

            var response = await _httpClient.GetAsync(uri);
            var listingsResponse = await response.Content.ReadAsAsync<ListingsResponse>();

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
                var response = await _httpClient.GetAsync(newUri);
                var listingsResponse = await response.Content.ReadAsAsync<ListingsResponse>();

                collection.AddRange(listingsResponse.listings);
            }

            return collection;
        }

        public static Uri GetTheShowUri(string baseUri, string endpoint, Dictionary<string, string> parameters)
        {
            var url = string.Format("{0}{1}", baseUri, endpoint);
            var newUri = new Uri(QueryHelpers.AddQueryString(url, parameters));

            return newUri;
        }
    }
}
