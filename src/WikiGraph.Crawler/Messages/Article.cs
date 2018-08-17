using System;
using System.Collections.Generic;

namespace WikiGraph.Crawler
{
    public class Article
    {
        public string Title { get; }
        public IList<Article> LinkedArticles { get; }

        public Article(string title, IList<Article> linkedArticles)
        {
            Title = title;
            LinkedArticles = linkedArticles;
        }
    }
}