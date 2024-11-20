using Fasciculus.IO;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Fasciculus.Core.Tests
{
    [TestClass]
    public class HostingTests
    {
        [TestMethod]
        public void TestSpecialPaths()
        {
            IHost host = ApplicationHost.CreateEmptyBuilder().UseSpecialPaths().Build();
            ISpecialPaths service = host.Services.GetRequiredService<ISpecialPaths>();

            Assert.IsNotNull(service);
        }

        [TestMethod]
        public void TestSpecialDirectories()
        {
            IHost host = ApplicationHost.CreateEmptyBuilder().UseSpecialDirectories().Build();
            ISpecialDirectories service = host.Services.GetRequiredService<ISpecialDirectories>();

            Assert.IsNotNull(service);
        }
    }
}
