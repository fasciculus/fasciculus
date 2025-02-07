using Fasciculus.Plugins.Testee;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

namespace Fasciculus.Plugins.Tests
{
    [TestClass]
    public class GenericPluginTests
    {
        [TestMethod]
        public void Test()
        {
            DateTime expected;
            DateTime actual;

            FileInfo pluginFile = PluginFile.GetTestee();
            using GenericPlugin<IPluginTestee> plugin = new(pluginFile);

            expected = pluginFile.LastWriteTimeUtc;
            actual = plugin.Target.Version;

            Assert.AreEqual(expected, actual);

            expected = DateTime.UtcNow;
            pluginFile.LastWriteTimeUtc = expected;
            actual = plugin.Target.Version;

            Assert.AreEqual(expected, actual);
        }
    }
}
