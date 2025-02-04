
#if DEBUG
using Fasciculus.IO.Searching;
using System.Diagnostics;
using System.IO;
using System.Linq;
#else
using System;
#endif

namespace Fasciculus
{
    public class TestsBase
    {
        protected virtual void Log(string? message)
        {
            message ??= "";

#if DEBUG
            Debug.WriteLine(message);
#else
            Console.WriteLine(message);
#endif
        }

        protected virtual DirectoryInfo GetProjectDirectory(string extension = ".csproj")
        {
            return FileSearch
                .Search($"*{extension}", SearchPath.WorkingDirectoryAndParents)
                .FirstOrDefault()
                ?.Directory!;
        }
    }
}
