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
        protected override void Configure(WebApplicationBuilder builder) { }

        protected override void Configure(WebApplication app)
        {
            app.UseBootstrapResources();
            app.UseKatexResources();
        }

        [TestMethod]
        public void TestBootstrap()
        {
            HttpResponse response = Invoke("/lib/bootstrap/dist/css/bootstrap.css");
            byte[] bytes = response.Body.ReadAllBytes();
            string content = Encoding.UTF8.GetString(bytes);

            Assert.AreEqual(200, response.StatusCode);
            Assert.AreEqual(281043, content.Length);
        }

        [TestMethod]
        public void TestKatex()
        {
            HttpResponse response = Invoke("/lib/katex/katex.css");
            byte[] bytes = response.Body.ReadAllBytes();
            string content = Encoding.UTF8.GetString(bytes);

            Assert.AreEqual(200, response.StatusCode);
            Assert.AreEqual(31329, content.Length);
        }
    }
}
