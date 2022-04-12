using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace MLBTheShowSharp.Services
{
    internal class HttpClientService
    {
        private ILogger _log;
        private HttpClient _httpClient;

        public HttpClientService(ILogger logger, HttpClient httpClient)
        {
            _log = logger ?? throw new ArgumentNullException(nameof(logger));
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        public async Task<T> GetResponseAsync<T>(Uri uri)
        {
            var response = await _httpClient.GetAsync(uri);
            var content = await response.Content.ReadAsAsync<T>();

            return content;
        }
    }
}