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
            }

            return _client;
        }
    }
}