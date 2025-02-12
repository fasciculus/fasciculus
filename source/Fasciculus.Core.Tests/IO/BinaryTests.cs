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

            binary.WriteSByte(-2);
            binary.WriteShort(2);

            using MemoryStream input = new MemoryStream(output.ToArray());
            binary = input;

            Assert.AreEqual(-2, binary.ReadSByte());
            Assert.AreEqual(2, binary.ReadShort());
        }
    }
}
