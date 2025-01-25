using Fasciculus.Collections;
using Fasciculus.IO.Searching;
using NuGet.Packaging.Core;
using NuGet.Versioning;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace Fasciculus.NuGet.Services
{
    public class DirectoryPackagesProvider : IDirectoryPackagesProvider
    {
        public const string DirectoryPackagesFileName = "Directory.Packages.props";

        public FileInfo? SearchPackagesFile(SearchPath? searchPath = null)
        {
            return FileSearch
                .Search(DirectoryPackagesFileName, searchPath ?? SearchPath.WorkingDirectoryAndParents)
                .FirstOrDefault();
        }

        public PackageIdentity[] GetPackages(FileInfo file)
        {
            XDocument document = XDocument.Load(file.FullName);

            return [.. GetPackageElements(document).Select(GetPackageIdentity).NotNull()];
        }

        public PackageIdentity[] GetPackages(SearchPath? searchPath = null)
        {
            FileInfo? file = SearchPackagesFile(searchPath);

            return file is null ? [] : GetPackages(file);
        }

        private static PackageIdentity? GetPackageIdentity(XElement element)
        {
            XAttribute? includeAttribute = element.Attribute("Include");
            XAttribute? versionAttribute = element.Attribute("Version");

            if (includeAttribute is not null && versionAttribute is not null)
            {
                if (NuGetVersion.TryParse(versionAttribute.Value, out NuGetVersion? version))
                {
                    string id = includeAttribute.Value;

                    return new PackageIdentity(id, version);
                }
            }

            return null;
        }

        private static IEnumerable<XElement> GetPackageElements(XDocument document)
        {
            XElement? root = document.Root;

            if (root is not null)
            {
                foreach (XElement itemGroup in root.Elements("ItemGroup"))
                {
                    foreach (XElement packageVersion in itemGroup.Elements("PackageVersion"))
                    {
                        yield return packageVersion;
                    }
                }
            }
        }
    }
}
