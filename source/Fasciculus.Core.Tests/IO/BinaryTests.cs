using Fasciculus.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace Fasciculus.Core.Tests.IO
{
    [TestClass]
    public class BinaryTests
    {
        [TestMethod]
        public void Test()
        {
            using MemoryStream output = new MemoryStream();
            BinaryRW binary = output;

            binary.WriteInt8(-2);
            binary.WriteInt16(2);

            using MemoryStream input = new MemoryStream(output.ToArray());
            binary = input;

            Assert.AreEqual(-2, binary.ReadInt8());
            Assert.AreEqual(2, binary.ReadInt16());
        }
    }
}
