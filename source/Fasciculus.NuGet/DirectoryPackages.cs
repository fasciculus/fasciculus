using Fasciculus.IO.Searching;
using NuGet.Packaging.Core;
using NuGet.Versioning;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace Fasciculus.NuGet
{
    public class DirectoryPackages : IEnumerable<PackageIdentity>
    {
        public const string DefaultFileName = "Directory.Packages.props";

        private readonly PackageIdentity[] packages;

        public int Count => packages.Length;

        public DirectoryPackages(IEnumerable<PackageIdentity> packages)
        {
            this.packages = [.. packages];
        }


        public IEnumerator<PackageIdentity> GetEnumerator()
            => packages.AsEnumerable().GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => packages.AsEnumerable().GetEnumerator();

        public static DirectoryPackages Load(IEnumerable<FileInfo> files)
        {
            Dictionary<string, PackageIdentity> packages = [];
            IEnumerable<PackageIdentity> candidates = [.. files.SelectMany(GetPackageCandidates)];

            foreach (PackageIdentity candidate in candidates)
            {
                packages.TryAdd(candidate.Id, candidate);
            }

            return new(packages.Values);
        }

        private static IEnumerable<PackageIdentity> GetPackageCandidates(FileInfo file)
        {
            XDocument document = XDocument.Load(file.FullName);
            XElement[] groups = [.. (document.Root?.Elements("ItemGroup") ?? [])];
            XElement[] entries = [.. groups.SelectMany(x => x.Elements("PackageVersion"))];

            foreach (XElement entry in entries)
            {
                XAttribute? includeAttribute = entry.Attribute("Include");
                XAttribute? versionAttribute = entry.Attribute("Version");

                if (includeAttribute is not null && versionAttribute is not null)
                {
                    if (NuGetVersion.TryParse(versionAttribute.Value, out NuGetVersion? version))
                    {
                        string id = includeAttribute.Value;

                        yield return new PackageIdentity(id, version);
                    }
                }
            }
        }

        public static DirectoryPackages Load(string fileName, SearchPath searchPath, bool includeParents = false)
            => Load(Search(fileName, searchPath, includeParents));

        public static DirectoryPackages Load(SearchPath searchPath, bool includeParents = false)
            => Load(DefaultFileName, searchPath, includeParents);

        public static DirectoryPackages Load(bool includeParents = false)
            => Load(SearchPath.WorkingDirectoryAndParents(), includeParents);

        public static FileInfo[] Search(string fileName, SearchPath searchPath, bool includeParents = false)
        {
            FileInfo[] files = [.. FileSearch.Search(fileName, searchPath)];
            int count = Math.Max(files.Length, includeParents ? files.Length : 1);

            return [.. files.Take(count)];
        }

        public static FileInfo[] Search(SearchPath searchPath, bool includeParents = false)
            => Search(DefaultFileName, searchPath, includeParents);

        public static FileInfo[] Search(bool includeParents = false)
            => Search(SearchPath.WorkingDirectoryAndParents(), includeParents);
    }
}
