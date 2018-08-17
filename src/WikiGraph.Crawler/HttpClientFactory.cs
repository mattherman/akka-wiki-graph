using System;
using System.Net.Http;

namespace WikiGraph.Crawler
{
    internal static class HttpClientFactory
    {
        private static HttpClient _client;

        internal static HttpClient GetClient()
        {
            if (_client == null)
            {
                _client = new HttpClient();
                _client.Timeout = TimeSpan.FromSeconds(5);
            }

            return _client;
        }
    }
}