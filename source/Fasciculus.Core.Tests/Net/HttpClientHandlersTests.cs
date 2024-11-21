using Fasciculus.Net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net;
using System.Net.Http;

namespace Fasciculus.Core.Tests.Net
{
    [TestClass]
    public class HttpClientHandlersTests : TestsBase
    {
        [TestMethod]
        public void TestCreate()
        {
            HttpClientHandlers httpClientHandlers = new();

            using HttpClientHandler httpClientHandler1 = new();
            using HttpClientHandler httpClientHandler2 = httpClientHandlers.CreateHttpClientHandler();
            DecompressionMethods decompressionMethods1 = DecompressionMethods.None;
            DecompressionMethods decompressionMethods2 = DecompressionMethods.GZip | DecompressionMethods.Deflate;

            Log($"{httpClientHandler1.AutomaticDecompression}");
            Log($"{httpClientHandler2.AutomaticDecompression}");

            Assert.AreEqual(decompressionMethods1, httpClientHandler1.AutomaticDecompression);
            Assert.AreEqual(decompressionMethods2, httpClientHandler2.AutomaticDecompression);
        }
    }
}
