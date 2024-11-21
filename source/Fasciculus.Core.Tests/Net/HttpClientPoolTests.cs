using Fasciculus.Net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Fasciculus.Core.Tests.Net
{
    [TestClass]
    public class HttpClientPoolTests
    {
        [TestMethod]
        public void TestCreate()
        {
            HttpClientHandlers httpClientHandlers = new();
            HttpClientPool httpClientPool = new(httpClientHandlers);

            using HttpClientHandler httpClientHandler1 = httpClientHandlers.CreateHttpClientHandler();
            using HttpClientHandler httpClientHandler2 = httpClientHandlers.CreateHttpClientHandler();

            using HttpClient httpClient1 = new(httpClientHandler1);
            using HttpClient httpClient2 = httpClientPool.CreateHttpClient(httpClientHandler2);

            HttpHeaderValueCollection<StringWithQualityHeaderValue> encodings1 = httpClient1.DefaultRequestHeaders.AcceptEncoding;
            HttpHeaderValueCollection<StringWithQualityHeaderValue> encodings2 = httpClient2.DefaultRequestHeaders.AcceptEncoding;

            string[] names1 = encodings1.Select(e => e.Value).ToArray();
            string[] names2 = encodings2.Select(e => e.Value).ToArray();

            Assert.AreEqual(0, names1.Length);
            Assert.AreEqual(2, names2.Length);
            Assert.IsTrue(names2.Contains("gzip"));
            Assert.IsTrue(names2.Contains("deflate"));
        }
    }
}
