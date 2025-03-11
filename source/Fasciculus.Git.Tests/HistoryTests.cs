using Fasciculus.IO;
using LibGit2Sharp;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Fasciculus.Git.Tests
{
    [TestClass]
    public class HistoryTests
    {
        [TestMethod]
        public void Test()
        {
            DirectoryInfo directory = DirectorySearch.Search(".git", SearchPath.WorkingDirectoryAndParents()).First();
            using Repository repository = new(directory.FullName);
            string path = "source/Fasciculus.Git.Tests/Fasciculus.Git.Tests.csproj";
            CommitFilter filter = new() { FirstParentOnly = true };
            IEnumerable<LogEntry> history = repository.Commits.QueryBy(path, filter);
            LogEntry? entry = history.FirstOrDefault();

            Assert.IsNotNull(entry);
        }
    }
}
