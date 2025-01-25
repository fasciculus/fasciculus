using Fasciculus.IO.Searching;
using NuGet.Packaging.Core;
using System.IO;

namespace Fasciculus.NuGet.Services
{
    public interface IDirectoryPackagesProvider
    {
        public FileInfo? SearchPackagesFile(SearchPath? searchPath = null);

        public PackageIdentity[] GetPackages(FileInfo file);

        public PackageIdentity[] GetPackages(SearchPath? searchPath = null);
    }
}
