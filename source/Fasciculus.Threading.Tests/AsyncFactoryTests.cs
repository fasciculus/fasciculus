using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace Fasciculus.Threading.Tests
{
    [TestClass]
    public class AsyncFactoryTests
    {
        private readonly InterlockedLong work1 = new();
        private readonly InterlockedLong work2 = new();

        private void Work1()
        {
            work1.Increment();
        }

        private async Task Work2()
        {
            await Task.Yield();

            work2.Increment();
        }

        private long Work3()
        {
            return 3;
        }

        private async Task<long> Work4()
        {
            await Task.Yield();

            return 4;
        }

        [TestMethod]
        public async Task TestAction()
        {
            IAsyncAction factory = AsyncFactory.Create(Work1);

            await factory.Create();

            Assert.AreEqual(1, work1.Read());
        }

        [TestMethod]
        public async Task TestAsyncAction()
        {
            IAsyncAction factory = AsyncFactory.Create(Work2);

            await factory.Create();

            Assert.AreEqual(1, work2.Read());
        }

        [TestMethod]
        public async Task TestFunc()
        {
            IAsyncFunc<long> factory = AsyncFactory.Create(Work3);
            long actual = await factory.Create();

            Assert.AreEqual(3, actual);
        }

        [TestMethod]
        public async Task TestAsyncFunc()
        {
            IAsyncFunc<long> factory = AsyncFactory.Create(Work4);
            long actual = await factory.Create();

            Assert.AreEqual(4, actual);
        }
    }
}
