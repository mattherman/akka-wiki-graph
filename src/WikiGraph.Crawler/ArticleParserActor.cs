using Akka.Actor;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace WikiGraph.Crawler
{
    public class ArticleParserActor : ReceiveActor
    {
        public class HtmlDocument
        {
            public string Content { get; }

            public HtmlDocument(string content)
            {
                Content = content;
            }
        }

        private readonly Regex _linkRegex;
        public ArticleParserActor()
        {
            _linkRegex = new Regex(@"^(/wiki/)((?!:).)*$");
            CollectLinks();
        }

        private void CollectLinks()
        {
            Receive<HtmlDocument>(page => {
                var doc = new HtmlAgilityPack.HtmlDocument();
                doc.LoadHtml(page.Content);

                var titleNode = doc.GetElementbyId("firstHeading");
                var parentTitle = titleNode.InnerText;

                var linkNodes = doc.GetElementbyId("bodyContent").SelectNodes("//a");
                
                var linkedArticlesMap = new Dictionary<string, Article>();
                foreach (var node in linkNodes)
                {
                    if (!node.Attributes.Contains("href") || !node.Attributes.Contains("title"))
                        continue;

                    var href = node.Attributes["href"].Value;
                    var title = node.Attributes["title"].Value;
                    if (_linkRegex.IsMatch(href) && !linkedArticlesMap.ContainsKey(title))
                    {
                        var uri = new Uri($"http://wikipedia.org{href}");
                        linkedArticlesMap.Add(title, new Article(title, new List<Article>()));
                    }
                }

                Context.Parent.Tell(new Article(parentTitle, linkedArticlesMap.Values.ToList()));
            });
        }
    }
}