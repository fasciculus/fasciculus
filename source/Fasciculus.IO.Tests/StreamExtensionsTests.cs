using Fasciculus.IO.Binary;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace Fasciculus.IO.Tests
{
    [TestClass]
    public class StreamExtensionsTests
    {
        [TestMethod]
        public void Test()
        {
            using MemoryStream stream = new();

            bool expectedBool = true;
            short expectedShort = 42;
            float expectedFloat = 3.14f;

            stream.WriteBool(expectedBool);
            stream.WriteInt16(expectedShort);
            stream.WriteSingle(expectedFloat);

            stream.Position = 0;

            Assert.AreEqual(expectedBool, stream.ReadBool());
            Assert.AreEqual(expectedShort, stream.ReadInt16());
            Assert.AreEqual(expectedFloat, stream.ReadSingle());
        }
    }
}
