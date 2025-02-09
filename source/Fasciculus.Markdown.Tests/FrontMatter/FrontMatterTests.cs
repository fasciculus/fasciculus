using Fasciculus.IO.Resources;
using Fasciculus.Markdown.FrontMatter;
using HtmlAgilityPack;
using Markdig;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Fasciculus.Markdown.Tests.FrontMatter
{
    [TestClass]
    public class FrontMatterTests
    {
        [TestMethod]
        public void Test()
        {
            MarkdownPipeline pipeline = new MarkdownPipelineBuilder()
                .UseYamlFrontMatter()
                .UseFrontMatter()
                .Build();

            string markdown = EmbeddedResources.Find("FrontMatter.Test1").ReadString();
            string html = Markdig.Markdown.ToHtml(markdown, pipeline);

            HtmlDocument document = new();

            document.LoadHtml(html);

            HtmlNode[] tables = [.. document.DocumentNode.Descendants("table")];

            Assert.AreEqual(1, tables.Length);

            HtmlNode[] rows = [.. tables[0].Descendants("tr")];

            Assert.AreEqual(2, rows.Length);

            HtmlNode[] cells;

            cells = [.. rows[0].Descendants("td")];

            Assert.AreEqual(2, cells.Length);
            Assert.AreEqual("Author", cells[0].InnerHtml);
            Assert.AreEqual("Roger H. Jörg", cells[1].InnerHtml);

            cells = [.. rows[1].Descendants("td")];

            Assert.AreEqual(2, cells.Length);
            Assert.AreEqual("Published", cells[0].InnerHtml);
            Assert.AreEqual("2025-02-08", cells[1].InnerHtml);
        }
    }
}
