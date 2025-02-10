using Fasciculus.IO;
using Fasciculus.Testing.Web;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text;

namespace Fasciculus.Web.Resources.Tests
{
    [TestClass]
    public class WebResourcesTests : WebTestsBase
    {
        protected override void Configure(WebApplicationBuilder builder)
        {

        }

        protected override void Configure(WebApplication app)
        {
            app.UseTestResources();
        }

        [TestMethod]
        public void TestFooAndBar()
        {
            HttpResponse response;
            byte[] bytes;
            string content;

            response = Invoke("/foo.txt");
            bytes = response.Body.ReadAllBytes();
            content = Encoding.UTF8.GetString(bytes);

            Assert.AreEqual(200, response.StatusCode);
            Assert.AreEqual("foo", content);

            response = Invoke("/foobar/bar.txt");
            bytes = response.Body.ReadAllBytes();
            content = Encoding.UTF8.GetString(bytes);

            Assert.AreEqual(200, response.StatusCode);
            Assert.AreEqual("bar", content);
        }
    }
}
