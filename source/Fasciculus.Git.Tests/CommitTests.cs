using Fasciculus.IO.Searching;
using LibGit2Sharp;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Linq;

namespace Fasciculus.Git.Tests
{
    [TestClass]
    public class CommitTests : TestsBase
    {
        [TestMethod]
        public void Test()
        {
            DirectoryInfo directory = DirectorySearch.Search(".git", SearchPath.WorkingDirectoryAndParents).First();
            using Repository repository = new(directory.FullName);
            IQueryableCommitLog commits = repository.Commits;

            foreach (Commit commit in commits.Take(15))
            {
                Log($"commit {commit.Id}");
                Log($"  Author : {commit.Author}");
                Log($"  When   : {commit.Author.When}");
            }
        }
    }
}
