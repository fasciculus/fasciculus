using Fasciculus.Collections;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Fasciculus.Site.Generating.Services
{
    public class Deleter
    {
        private readonly DirectoryInfo outputDirectory;
        private readonly GeneratorWriter writer;

        public Deleter([FromKeyedServices(GeneratorWriter.OutputDirectoryKey)] DirectoryInfo outputDirectory, GeneratorWriter writer)
        {
            this.outputDirectory = outputDirectory;
            this.writer = writer;
        }

        public void Run()
        {
            SortedSet<string> toDelete = GetExistingFiles();

            toDelete.ExceptWith(writer.AllFiles);

            Debug.WriteLine($"~~~ FILES TO DELETE ({toDelete.Count}) ~~~");

            toDelete.Apply(f => { Debug.WriteLine(f); });

            Debug.WriteLine("~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
        }

        private SortedSet<string> GetExistingFiles()
        {
            string outputDirectoryPath = outputDirectory.FullName;

            IEnumerable<string> paths = outputDirectory
                .GetFiles("*", SearchOption.AllDirectories)
                .Select(f => Path.GetRelativePath(outputDirectoryPath, f.FullName))
                .Select(f => f.Replace('\\', '/'))
                .Where(p => !p.StartsWith(".git"));

            return new(paths);
        }
    }
}
