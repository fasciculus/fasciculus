using HtmlAgilityPack;
using System;
using System.Diagnostics;

namespace Fasciculus.Experiments.Html
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string snippet = "<p><b>foo:</b> bar</p>";
            HtmlNode node = HtmlNode.CreateNode(snippet);
            string result = node.OuterHtml;

            Console.WriteLine(result);
            Debug.WriteLine(result);
        }
    }
}
