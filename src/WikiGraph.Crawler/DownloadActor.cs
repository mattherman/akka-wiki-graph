using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Akka.Actor;

namespace WikiGraph.Crawler
{
    public class DownloadActor : ReceiveActor
    {
        public class PageDownloadResult
        {
            public bool Success { get; }
            public byte[] Content { get; }

            public PageDownloadResult(bool success, byte[] content)
            {
                Success = success;
                Content = content;
            }
        }

        private IHttpClientFactory _httpClientFactory;

        public DownloadActor(IHttpClientFactory clientFactory)
        {
            _httpClientFactory = clientFactory;
            ReadyToDownload();
        }

        private void ReadyToDownload()
        {
            Receive<Uri>(uri => {

                Become(Downloading);

                using(var client = _httpClientFactory.CreateClient()) 
                {
                    client.GetAsync(uri).ContinueWith(httpRequest =>
                    {
                        var response = httpRequest.Result;

                        if (response.StatusCode == HttpStatusCode.OK)
                        {
                            try
                            {
                                var pageBytes = response.Content.ReadAsByteArrayAsync().Result;
                                return new PageDownloadResult(true, pageBytes);
                            }
                            catch //timeout exceptions!
                            {
                                return new PageDownloadResult(false, new byte[0]);
                            }
                        }

                        return new PageDownloadResult(false, new byte[0]);
                    }, TaskContinuationOptions.AttachedToParent & TaskContinuationOptions.ExecuteSynchronously).PipeTo(Self);
                }
            });
        }

        private void Downloading()
        {
            Receive<PageDownloadResult>(result => {

            });
        }
    }
}