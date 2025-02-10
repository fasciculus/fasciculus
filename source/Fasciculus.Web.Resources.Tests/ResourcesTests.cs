using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Reflection;

namespace Fasciculus.Web.Resources.Tests
{
    [TestClass]
    public class ResourcesTests
    {
        [TestMethod]
        public void Test()
        {
            Assembly assembly = typeof(WebResources).Assembly;
            string[] names = assembly.GetManifestResourceNames();

            Assert.AreEqual(2, names.Length);
            Assert.IsTrue(names.Contains("Fasciculus.Web.Resources.Test.foo.txt"));
            Assert.IsTrue(names.Contains("Fasciculus.Web.Resources.Test.foobar.bar.txt"));
        }
    }
}
