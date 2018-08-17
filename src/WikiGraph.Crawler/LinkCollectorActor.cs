using Akka.Actor;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;

namespace WikiGraph.Crawler
{
    public class LinkCollectorActor : ReceiveActor
    {
        public LinkCollectorActor()
        {
            CollectLinks();
        }

        private void CollectLinks()
        {
            Receive<HtmlDocument>(doc => {
                var nodes = doc.DocumentNode.SelectNodes("//a");
                
                var linkedArticles = new List<Article>();
                foreach (var node in nodes)
                {
                    if (!node.Attributes.Contains("href"))
                        continue;

                    var href = node.Attributes["href"].Value;
                    if (href.StartsWith("/wiki/") && !href.Contains(":"))
                    {
                        var uri = new Uri($"http://wikipedia.org{href}");
                        var name = node.Attributes.Contains("title") ? node.Attributes["title"].Value : uri.Segments[2];
                        linkedArticles.Add(new Article(name, uri));
                    }
                }

                Context.Parent.Tell(linkedArticles);
            });
        }
    }
}