using Fasciculus.IO;
using Fasciculus.Net.Navigating;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Linq;

namespace Fasciculus.Core.Tests.IO
{
    [TestClass]
    public class RelativeTests
    {
        [TestMethod]
        public void Experiment()
        {
            SearchPath searchPath = SearchPath.WorkingDirectoryAndParents();
            DirectoryInfo directory = DirectorySearch.Search("Fasciculus.Core.Tests", searchPath).First();
            FileInfo file = directory.Combine("IO").File("RelativeTests.cs");

            Uri directoryUri = new(directory.FullName);
            Uri fileUri = new(file.FullName);

            Uri relativeUri = directoryUri.MakeRelativeUri(fileUri);
            string[] parts = relativeUri.OriginalString.Split('/');

            UriPath expected = new("IO", "RelativeTests.cs");
            UriPath actual = new(parts.Skip(1));

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestRelativeTo()
        {
            SearchPath searchPath = SearchPath.WorkingDirectoryAndParents();
            DirectoryInfo directory = DirectorySearch.Search("Fasciculus.Core.Tests", searchPath).First();
            FileInfo file = directory.Combine("IO").File("RelativeTests.cs");

            UriPath expected = new("IO", "RelativeTests.cs");
            UriPath actual = new(file.RelativeTo(directory));

            Assert.AreEqual(expected, actual);
        }
    }
}
