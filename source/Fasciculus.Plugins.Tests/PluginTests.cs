using Fasciculus.Plugins.Testee;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

namespace Fasciculus.Plugins.Tests
{
    [TestClass]
    public class PluginTests
    {
        [TestMethod]
        public void Test()
        {
            FileInfo pluginFile = PluginFile.GetTestee();
            DateTime expected = pluginFile.LastWriteTimeUtc;
            DateTime actual = Plugin.Select<IPluginTestee, DateTime>(pluginFile, p => p.Version);

            Assert.AreEqual(expected, actual);
        }
    }
}
