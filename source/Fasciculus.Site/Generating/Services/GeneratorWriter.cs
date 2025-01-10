using Fasciculus.IO;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.IO;

namespace Fasciculus.Site.Generating.Services
{
    public class GeneratorWriter
    {
        public const string OutputDirectoryKey = "_OutputDirectory";

        private readonly DirectoryInfo outputDirectory;

        private readonly SortedSet<string> allFiles = [];
        private readonly SortedSet<string> modifiedFiles = [];

        public IReadOnlySet<string> AllFiles => allFiles;
        public IReadOnlySet<string> ModifiedFiles => modifiedFiles;

        public GeneratorWriter([FromKeyedServices(OutputDirectoryKey)] DirectoryInfo outputDirectory)
        {
            this.outputDirectory = outputDirectory;
        }

        public void Write(string document, string contentType, byte[] content)
        {
            if (document.EndsWith("/"))
            {
                document += "index.html";
            }

            if (contentType.StartsWith("text/html") && !document.EndsWith(".html"))
            {
                document += ".html";
            }

            if (document.StartsWith("/"))
            {
                document = document[1..];
            }

            FileInfo file = outputDirectory.File(document);

            file.WriteIfDifferent(content, out bool written);

            allFiles.Add(document);

            if (written)
            {
                modifiedFiles.Add(file.FullName);
            }
        }
    }
}
