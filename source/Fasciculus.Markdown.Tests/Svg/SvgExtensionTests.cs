using Fasciculus.Markdown.Svg;
using Fasciculus.Svg.Elements;
using Markdig;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Fasciculus.Markdown.Tests.Extensions.Svg
{
    [TestClass]
    public class SvgExtensionTests
    {
        [TestMethod]
        public void Test()
        {
            SvgMappings mappings = new();
            SvgSvg svg = SvgSvg.Create(20, 10);

            mappings.Add("Test", svg);

            MarkdownPipeline pipeline = new MarkdownPipelineBuilder()
                .UseSvg(mappings)
                .Build();

            string markdown = "# Test\r\n\r\n!svg{Test, 20px, 10px}";
            string html = Markdig.Markdown.ToHtml(markdown, pipeline);

            string expected = "<h1>Test</h1>\n"
                + "<svg xmlns=\"http://www.w3.org/2000/svg\" viewBox=\"0 0 20 10\" width=\"20px\" height=\"10px\" />\n";

            Assert.AreEqual(expected, html);
        }
    }
}
