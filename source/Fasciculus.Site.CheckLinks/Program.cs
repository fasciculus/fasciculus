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
            SearchPath searchPath = SearchPath.WorkingDirectoryAndParents();
            DirectoryInfo directory = DirectorySearch.Search("fasciculus.github.io", searchPath).First();
            FileInfo[] files = directory.GetFiles("*.html", SearchOption.AllDirectories);
            SortedSet<string> links = [];

            Log("parsing documents");

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

            string[] externalLinks = links
                .Where(x => x.StartsWith("http"))
                .Where(x => !x.StartsWith("https://github.com/fasciculus/fasciculus/"))
                .Where(x => !x.StartsWith("https://www.nuget.org/"))
                .ToArray();

            Log("checking links");

            using HttpClient httpClient = HttpClientFactory.Create(null);
            int broken = externalLinks.AsParallel().Select(link => CheckLink(httpClient, link)).Count(x => !x);

            Log($"{broken} / {externalLinks.Length} broken links");
        }

        private static bool CheckLink(HttpClient httpClient, string link)
        {
            try
            {
                httpClient.Head(new(link));

                return true;
            }
            catch
            {
                Log($"broken: {link}");
            }

            return false;
        }

        private static void Log(string message)
        {
            Console.WriteLine(message);
            Debug.WriteLine(message);
        }
    }
}
