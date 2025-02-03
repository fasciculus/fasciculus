using Fasciculus.Collections;
using Fasciculus.IO;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Fasciculus.Site.Generating.Services
{
    public class GeneratorDeleter
    {
        private readonly DirectoryInfo outputDirectory;
        private readonly GeneratorWriter writer;

        public GeneratorDeleter([FromKeyedServices(GeneratorWriter.OutputDirectoryKey)] DirectoryInfo outputDirectory, GeneratorWriter writer)
        {
            this.outputDirectory = outputDirectory;
            this.writer = writer;
        }

        public void Run()
        {
            SortedSet<string> toDelete = GetExistingFiles();

            toDelete.ExceptWith(writer.AllFiles);

            Console.WriteLine($"~~~ FILES TO DELETE ({toDelete.Count}) ~~~");
            Debug.WriteLine($"~~~ FILES TO DELETE ({toDelete.Count}) ~~~");

            toDelete.Apply(Delete);

            Console.WriteLine("~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
            Debug.WriteLine("~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
        }

        private void Delete(string relativePath)
        {
            Console.WriteLine(relativePath);
            Debug.WriteLine(relativePath);

            FileInfo file = outputDirectory.File(relativePath);

            file.Delete();

            DirectoryInfo directory = file.Directory!;

            if (directory.GetDirectories().Length == 0)
            {
                if (directory.GetFiles().Length == 0)
                {
                    directory.Delete(false);
                }
            }
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
