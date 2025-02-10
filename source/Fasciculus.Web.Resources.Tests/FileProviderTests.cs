using Microsoft.Extensions.FileProviders;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace Fasciculus.Web.Resources.Tests
{
    [TestClass]
    public class FileProviderTests
    {
        [TestMethod]
        public void TestBootstrap()
        {
            IFileProvider provider = WebResources.BootstrapProvider;
            IDirectoryContents contents;

            contents = provider.GetDirectoryContents("");
            Assert.AreEqual(45, contents.Count());
        }
    }
}
