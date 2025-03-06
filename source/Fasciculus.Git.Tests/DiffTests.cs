using Fasciculus.IO;
using LibGit2Sharp;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Fasciculus.Git.Tests
{
    [TestClass]
    public class DiffTests
    {
        [TestMethod]
        public void Test()
        {
            DirectoryInfo directory = DirectorySearch.Search(".git", SearchPath.WorkingDirectoryAndParents()).First();
            using Repository repository = new(directory.FullName);
            IQueryableCommitLog commits = repository.Commits;
            Commit lastCommit = commits.First();
            Commit firstCommit = commits.Last();
            TreeChanges allChanges = repository.Diff.Compare<TreeChanges>(firstCommit.Tree, lastCommit.Tree);
            IEnumerable<TreeEntryChanges> sourceChanges = allChanges.Where(x => x.Path.StartsWith("source/"));
            IEnumerable<string> sourcePaths = sourceChanges.Select(x => x.Path[7..]).Distinct();
            IEnumerable<string> projects = sourcePaths.Where(x => x.Contains('/')).Select(x => x[..x.IndexOf('/')]).Distinct();

            Assert.IsTrue(projects.Any());
        }
    }
}
