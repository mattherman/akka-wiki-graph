using Akka.Actor;
using WikiGraph.Crawler.Messages;

namespace WikiGraph.Crawler
{
    public class CrawlActor : ReceiveActor
    {
        private IActorRef _downloadActor;
        
        public CrawlActor()
        {
            _downloadActor = Context.ActorOf(Props.Create(() => new DownloadActor()), "download");
            AcceptingCrawlJobs();
        }

        private void AcceptingCrawlJobs()
        {
            Receive<CrawlJob>(job => {
                // do the stuff
            });
        }
    }
}