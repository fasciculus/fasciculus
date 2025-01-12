using Fasciculus.IO.Searching;
using Fasciculus.Net;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;

namespace Fasciculus.Site.CheckLinks
{
    public static class Program
    {
        static void Main(string[] args)
        {
            SearchPath searchPath = SearchPath.WorkingDirectoryAndParents;
            DirectoryInfo directory = DirectorySearch.Search("fasciculus.github.io", searchPath).First();
            FileInfo[] files = directory.GetFiles("*.html", SearchOption.AllDirectories);
            SortedSet<string> links = [];

            foreach (FileInfo file in files)
            {
                HtmlDocument document = new();

                document.Load(file.FullName);

                foreach (HtmlNode link in document.DocumentNode.SelectNodes("//a[@href]"))
                {
                    HtmlAttribute attribute = link.Attributes["href"];

                    links.Add(attribute.Value);
                }
            }

            string[] externalLinks = [.. links.Where(x => x.StartsWith("http"))];
            using HttpClient httpClient = HttpClientFactory.Create(null);

            foreach (string externalLink in externalLinks)
            {
                try
                {
                    httpClient.Head(new(externalLink));
                }
                catch
                {
                    string message = $"broken: {externalLink}";

                    Console.WriteLine(message);
                    Debug.WriteLine(message);
                }
            }
        }
    }
}
