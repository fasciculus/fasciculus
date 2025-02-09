using Fasciculus.IO.Resources;
using Fasciculus.Markdown.FrontMatter;
using Markdig;
using Markdig.Syntax;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Fasciculus.Markdown.Tests.FrontMatter
{
    [TestClass]
    public class FrontMatterParserTests
    {
        [TestMethod]
        public void Test()
        {
            FrontMapperMappings mappings = new();

            MarkdownPipeline pipeline = new MarkdownPipelineBuilder()
                .UseYamlFrontMatter()
                .UseFrontMatter(mappings)
                .Build();

            string markdown = EmbeddedResources.Find("FrontMatter.Test1").ReadString();
            MarkdownDocument document = Markdig.Markdown.Parse(markdown, pipeline);
            FrontMatterBlock[] blocks = [.. document.Descendants<FrontMatterBlock>()];

            Assert.AreEqual(1, blocks.Length);

            string[] expected = { "Author", "Published" };
            string[] actual = [.. blocks[0].Keys];

            CollectionAssert.AreEqual(expected, actual);
        }
    }
}
