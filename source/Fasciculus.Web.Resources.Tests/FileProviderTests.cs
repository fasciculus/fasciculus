using Fasciculus.Web.Extensions;
using Microsoft.Extensions.FileProviders;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace Fasciculus.Web.Resources.Tests
{
    [TestClass]
    public class FileProviderTests
    {
        [TestMethod]
        public void Test()
        {
            EmbeddedFileProvider provider = WebResources.BootstrapProvider;

            Assert.AreEqual(45, provider.GetDirectoryContents("").Count());
            Assert.AreEqual(275, provider.GetSubPaths().Count());
        }
    }
}
