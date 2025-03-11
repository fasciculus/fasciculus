using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Threading.Tasks;

namespace Fasciculus.Threading.Tests
{
    [TestClass]
    public class AsyncLazyTests
    {
        public class Foo { }

        private static Foo CreateFoo()
            => new();

        private static async Task<Foo> CreateFooAsync()
        {
            await Task.Yield();

            return new();
        }

        private readonly AsyncLazy<Foo> lazy1 = new(CreateFoo);
        private readonly AsyncLazy<Foo> lazy2 = new(CreateFooAsync);

        private async Task<Foo> GetValue1()
        {
            return await lazy1;
        }

        private async Task<Foo> GetValue2()
        {
            return await lazy2;
        }

        [TestMethod]
        public void TestFactory()
        {
            int count = 4;
            Task<Foo>[] tasks = [.. Enumerable.Range(0, count).Select(_ => GetValue1())];
            Foo[] foos = Tasks.Wait(tasks);

            for (int i = 1; i < count; ++i)
            {
                Assert.AreSame(foos[0], foos[i]);
            }
        }

        [TestMethod]
        public void TestAsyncFactory()
        {
            int count = 4;
            Task<Foo>[] tasks = [.. Enumerable.Range(0, count).Select(_ => GetValue2())];
            Foo[] foos = Tasks.Wait(tasks);

            for (int i = 1; i < count; ++i)
            {
                Assert.AreSame(foos[0], foos[i]);
            }
        }
    }
}
