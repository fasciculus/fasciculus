using Fasciculus.IO.Searching;
using Fasciculus.PackageVersions.Models;
using NuGet.Versioning;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace Fasciculus.PackageVersions.Services
{
    public class PackagesProvider
    {
        public static List<PackageInfo> GetPackages()
        {
            return new(GetPackageElements().Select(CreatePackageInfo));
        }

        private static PackageInfo CreatePackageInfo(XElement element)
        {
            string name = element.Attribute("Include")!.Value;
            NuGetVersion version = NuGetVersion.Parse(element.Attribute("Version")!.Value);

            return new(name, version);
        }

        private static IEnumerable<XElement> GetPackageElements()
        {
            XDocument document = XDocument.Load(GetPackagesFile().FullName);

            foreach (XElement group in document.Root!.Elements("ItemGroup"))
            {
                foreach (XElement package in group.Elements("PackageVersion"))
                {
                    yield return package;
                }
            }
        }

        private static FileInfo GetPackagesFile()
        {
            SearchPath searchPath = SearchPath.WorkingDirectoryAndParents;

            return FileSearch.Search("Directory.Packages.props", searchPath).First();
        }
    }
}
