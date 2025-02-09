using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using YamlDotNet.RepresentationModel;
using YamlDotNet.Serialization;

namespace Fasciculus.Markdown.Tests.FrontMatter
{
    [TestClass]
    public class FrontMatterYamlTests
    {
        private const string YamlText = "title: \"Test\"\r\npublished: 2025-02-01\r\n";

        [TestMethod]
        public void YamlDictionaryTest()
        {
            IDeserializer deserializer = new DeserializerBuilder().Build();

            object? yaml = deserializer.Deserialize(YamlText);
            IDictionary<object, object>? dict = yaml as IDictionary<object, object>;

            Assert.IsNotNull(yaml);
            Assert.IsNotNull(dict);
        }

        [TestMethod]
        public void YamlStringDictionaryTest()
        {
            IDeserializer deserializer = new DeserializerBuilder().Build();
            Dictionary<string, string>? entries = deserializer.Deserialize<Dictionary<string, string>>(YamlText);

            Assert.IsNotNull(entries);
            Assert.AreEqual(2, entries.Count);
            Assert.IsTrue(entries.ContainsKey("title"));
            Assert.AreEqual("Test", entries["title"]);
            Assert.IsTrue(entries.ContainsKey("published"));
            Assert.AreEqual("2025-02-01", entries["published"]);
        }

        [TestMethod]
        public void YamlDocumentTest()
        {
            YamlDocument document = new(YamlText);

            YamlNode[] allNodes = [.. document.AllNodes];
            YamlNode rootNode = document.RootNode;

            YamlNode[] subNodes = [.. rootNode.AllNodes];
        }
    }
}
