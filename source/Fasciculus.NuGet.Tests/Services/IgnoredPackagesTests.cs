using Fasciculus.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using System.IO;

namespace Fasciculus.NuGet.Tests.Services
{
    [TestClass]
    public class IgnoredPackagesTests
    {
        [TestMethod]
        public void ListPackages()
        {
            DirectoryInfo fwk = SpecialDirectories.ProgramFiles
                .Combine("dotnet", "packs", "Microsoft.NETCore.App.Ref", "9.0.2", "ref", "net9.0");

            Assert.IsTrue(fwk.Exists);

            FileInfo[] dlls = fwk.GetFiles("*.dll", SearchOption.TopDirectoryOnly);

            foreach (FileInfo dll in dlls)
            {
                string name = dll.Name[..^4];

                Debug.WriteLine($"            ignored.Add(\"{name}\");");
            }
        }
    }
}
