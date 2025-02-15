using Fasciculus.NetStandard.Testee;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Reflection;

namespace Fasciculus.NetStandard.Tests
{
    [TestClass]
    public class TypesTests
    {
        [TestMethod]
        public void Test()
        {
            Assembly assembly = typeof(NetStandardTestee).Assembly;
            Type[] types = assembly.GetExportedTypes();

            Assert.AreEqual(1, types.Length);
        }
    }
}
