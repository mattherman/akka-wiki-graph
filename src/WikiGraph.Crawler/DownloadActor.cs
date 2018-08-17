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
            public string Content { get; }

            public PageDownloadResult(bool success, string content)
            {
                Success = success;
                Content = content;
            }
        }

        public DownloadActor()
        {
            ReadyToDownload();
        }

        private void ReadyToDownload()
        {
            Receive<Uri>(uri => {

                Become(Downloading);

                var client = HttpClientFactory.GetClient();

                // TODO: Update this to follow the `ContinueWith().PipeTo()` pattern
                var response = client.GetAsync(uri).Result;
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    try
                    {
                        var pageBytes = response.Content.ReadAsStringAsync().Result;
                        Self.Tell(new PageDownloadResult(true, pageBytes));
                    }
                    catch //timeout exceptions!
                    {
                        Self.Tell(new PageDownloadResult(false, string.Empty));
                    }
                }
                Self.Tell(new PageDownloadResult(false, string.Empty));
            });
        }

        private void Downloading()
        {
            Receive<PageDownloadResult>(result => {
                Become(ReadyToDownload);
                Context.Parent.Tell(result);
            });
        }
    }
}