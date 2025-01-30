using Fasciculus.IO.Resources;
using HtmlAgilityPack;
using Markdig;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Fasciculus.Markup.Tests.Markdown.FrontMatter
{
    [TestClass]
    public class FrontMatterTests
    {
        [TestMethod]
        public void Test()
        {
            string markdown = EmbeddedResources.Find("FrontMatterTest1").ReadString();

            MarkdownPipeline pipeline = new MarkdownPipelineBuilder()
                .UseYamlFrontMatter()
                .Build();

            string html = Markdig.Markdown.ToHtml(markdown, pipeline);
            HtmlDocument htmlDocument = new();

            htmlDocument.LoadHtml(html);
        }
    }
}
