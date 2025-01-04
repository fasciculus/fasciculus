using Fasciculus.IO;
using Microsoft.Extensions.DependencyInjection;
using System.IO;

namespace Fasciculus.GitHub.Services
{
    public class Writer
    {
        public const string OutputDirectoryKey = "_OutputDirectory";

        private readonly DirectoryInfo outputDirectory;

        public Writer([FromKeyedServices(OutputDirectoryKey)] DirectoryInfo outputDirectory)
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

            file.WriteIfDifferent(content);
        }
    }
}
