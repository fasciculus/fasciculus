using Fasciculus.Collections;
using Fasciculus.IO.Searching;
using NuGet.Frameworks;
using NuGet.Packaging.Core;
using NuGet.Versioning;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace Fasciculus.NuGet.Services
{
    public class ProjectPackagesProvider : IProjectPackagesProvider
    {
        public PackageIdentity[] GetPackages(IEnumerable<string> projectNames, SearchPath searchPath)
        {
            return GetPackages(projectNames
                .Select(x => x + ".csproj")
                .Select(x => FileSearch.Search(x, searchPath).FirstOrDefault())
                .NotNull());
        }

        public PackageIdentity[] GetPackages(IEnumerable<FileInfo> projectFiles)
        {
            return [.. projectFiles.SelectMany(GetPackages).Distinct()];
        }

        private IEnumerable<PackageIdentity> GetPackages(FileInfo projectFile)
        {
            XDocument document = XDocument.Load(projectFile.FullName);

            return GetPackageElements(document).Select(GetPackageIdentity).NotNull();
        }

        private static PackageIdentity? GetPackageIdentity(XElement element)
        {
            XAttribute? includeAttribute = element.Attribute("Include");

            if (includeAttribute is not null)
            {
                string id = includeAttribute.Value;
                NuGetVersion version = GetVersion(element);

                return new(id, version);
            }

            return null;
        }

        private static NuGetVersion GetVersion(XElement element)
        {
            XAttribute? versionAttribute = element.Attribute("Version");

            if (versionAttribute is not null)
            {
                if (NuGetVersion.TryParse(versionAttribute.Value, out NuGetVersion? version))
                {
                    return version;
                }
            }

            return new(FrameworkConstants.EmptyVersion);
        }

        private static IEnumerable<XElement> GetPackageElements(XDocument document)
        {
            XElement? root = document.Root;

            if (root is not null)
            {
                foreach (XElement itemGroup in root.Elements("ItemGroup"))
                {
                    foreach (XElement packageReference in itemGroup.Elements("PackageReference"))
                    {
                        yield return packageReference;
                    }
                }
            }
        }
    }
}
