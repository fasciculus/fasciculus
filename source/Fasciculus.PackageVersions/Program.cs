using Fasciculus.IO.Searching;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace Fasciculus.PackageVersions
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            GetPackages();
        }

        private static IEnumerable<PackageInfo> GetPackages()
        {
            SearchPath searchPath = SearchPath.WorkingDirectoryAndParents;
            FileInfo file = FileSearch.Search("Directory.Packages.props", searchPath).First();
            XDocument document = XDocument.Load(file.FullName);

            foreach (XElement groupElement in document.Root!.Elements("ItemGroup"))
            {
                foreach (XElement packageElement in groupElement.Elements("PackageVersion"))
                {
                    string name = packageElement.Attribute("Include")!.Value;
                    string version = packageElement.Attribute("Version")!.Value;
                }
            }

            return [];
        }
    }
}
