using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Fasciculus.Core.Tests
{
    [TestClass]
    public class MiscTests
    {
        private static readonly Random random = new(0);

        private static int Work(int index)
        {
            int delay = random.Next(100);

            Task.Delay(delay).Wait();

            return index;
        }

        [TestMethod]
        public void Test()
        {
            int[] expected = Enumerable.Range(0, 100).ToArray();
            int[] actual = expected.AsParallel().Select(Work).ToArray();

            CollectionAssert.AreEqual(expected, actual);
        }
    }
}
