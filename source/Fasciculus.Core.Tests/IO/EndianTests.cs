using Fasciculus.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Fasciculus.Core.Tests.IO
{
    [TestClass]
    public class EndianTests
    {
        private readonly byte[] buffer = new byte[8];

        //[TestMethod]
        //public void TestSByteLittle()
        //{
        //    sbyte input = -2;

        //    Endian.Little.SetSByte(buffer, input);

        //    sbyte output = Endian.Little.GetSByte(buffer);

        //    Assert.AreEqual(input, output);
        //}

        [TestMethod]
        public void TestSetShortLittle()
        {
            short input = 2;

            Endian.Little.SetShort(buffer, input);

            Assert.AreEqual(2, buffer[0]);
            Assert.AreEqual(0, buffer[1]);
        }

        [TestMethod]
        public void TestSetShortBig()
        {
            short input = 2;

            Endian.Big.SetShort(buffer, input);

            Assert.AreEqual(0, buffer[0]);
            Assert.AreEqual(2, buffer[1]);
        }

        [TestMethod]
        public void TestGetShortLittle()
        {
            buffer[0] = 2;
            buffer[1] = 0;

            short value = Endian.Little.GetShort(buffer);

            Assert.AreEqual(2, value);
        }

        [TestMethod]
        public void TestGetShortBig()
        {
            buffer[0] = 0;
            buffer[1] = 2;

            short value = Endian.Big.GetShort(buffer);

            Assert.AreEqual(2, value);
        }
    }
}
