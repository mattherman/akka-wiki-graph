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

        public class ArticleParseResult
        {
            public string Title { get; }
            public ICollection<string> LinkedArticles { get; }

            public ArticleParseResult(string title, ICollection<string> linkedArticles)
            {
                Title = title;
                LinkedArticles = linkedArticles;
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
                
                var linkedArticles = new HashSet<string>();
                foreach (var node in linkNodes)
                {
                    if (!node.Attributes.Contains("href") || !node.Attributes.Contains("title"))
                        continue;

                    var href = node.Attributes["href"].Value;
                    var title = node.Attributes["title"].Value;
                    if (_linkRegex.IsMatch(href))
                    {
                        linkedArticles.Add(title);
                    }
                }

                Context.Parent.Tell(new ArticleParseResult(parentTitle, linkedArticles));
            });
        }
    }
}