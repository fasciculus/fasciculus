using Fasciculus.IO.Searching;
using Microsoft.CodeAnalysis;
using System;
using System.IO;
using System.Linq;

namespace Fasciculus.Experiments.ApiDoc
{
    public static class Program
    {
        public static void Main()
        {
            FileInfo solutionFile = FileSearch.Search("fasciculus.sln", SearchPath.WorkingDirectoryAndParents).First();
            string[] solutionLines = [.. solutionFile.ReadAllLines().Select(x => x.Trim())];
            string solutionGuidLine = solutionLines.First(x => x.StartsWith("SolutionGuid"));
            Guid solutionGuid = Guid.Parse(solutionGuidLine.AsSpan("SolutionGuid = ".Length));
            SolutionId solutionId = SolutionId.CreateFromSerialized(solutionGuid);
            SolutionInfo solutionInfo = SolutionInfo.Create(solutionId, VersionStamp.Default, solutionFile.FullName);
            AdhocWorkspace workspace = new();
            Solution solution = workspace.AddSolution(solutionInfo);
        }
    }
}
