using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading;
using System.Threading.Tasks;

namespace Fasciculus.Threading.Tests
{
    [TestClass]
    public class AsyncFactoryTests
    {
        private long work1;
        private long work2;

        private void Work1()
        {
            Interlocked.Increment(ref work1);
        }

        private async Task Work2()
        {
            await Task.Yield();

            Interlocked.Increment(ref work2);
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
        public void TestAction()
        {
            IAsyncAction factory = AsyncFactory.Create(Work1);
            Task task = factory.Create();
            Tasks.Wait(task);
            long actual = Interlocked.Read(ref work1);

            Assert.AreEqual(1, actual);
        }

        [TestMethod]
        public void TestAsyncAction()
        {
            IAsyncAction factory = AsyncFactory.Create(Work2);
            Task task = factory.Create();
            Tasks.Wait(task);
            long actual = Interlocked.Read(ref work2);

            Assert.AreEqual(1, actual);
        }

        [TestMethod]
        public void TestFunc()
        {
            IAsyncFunc<long> factory = AsyncFactory.Create(Work3);
            Task<long> task = factory.Create();
            long actual = Tasks.Wait(task);

            Assert.AreEqual(3, actual);
        }

        [TestMethod]
        public void TestAsyncFunc()
        {
            IAsyncFunc<long> factory = AsyncFactory.Create(Work4);
            Task<long> task = factory.Create();
            long actual = Tasks.Wait(task);

            Assert.AreEqual(4, actual);
        }
    }
}
