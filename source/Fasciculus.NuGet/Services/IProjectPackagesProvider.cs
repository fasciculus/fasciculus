using Fasciculus.IO.Searching;
using NuGet.Packaging.Core;
using System.Collections.Generic;
using System.IO;

namespace Fasciculus.NuGet.Services
{
    public interface IProjectPackagesProvider
    {
        public PackageIdentity[] GetPackages(IEnumerable<string> projectNames, SearchPath searchPath);

        public PackageIdentity[] GetPackages(IEnumerable<FileInfo> projectFiles);
    }
}
