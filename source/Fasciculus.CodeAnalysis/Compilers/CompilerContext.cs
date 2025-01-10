using Fasciculus.CodeAnalysis.Frameworking;
using System.IO;

namespace Fasciculus.CodeAnalysis.Compilers
{
    public class CompilerContext
    {
        public TargetFramework Framework { get; private set; }

        public TargetFrameworks Frameworks => new(Framework);

        public string Package { get; private set; } = string.Empty;

        public DirectoryInfo? ProjectDirectory { get; private set; }

        public CompilerContext(TargetFramework framework, DirectoryInfo? projectDirectory)
        {
            Framework = framework;
            ProjectDirectory = projectDirectory;
        }

        private CompilerContext(CompilerContext other)
        {
            Framework = other.Framework;
            ProjectDirectory = other.ProjectDirectory;
        }

        public CompilerContext WithFramework(TargetFramework framework)
            => new(this) { Framework = framework };

        public CompilerContext WithPackage(string package)
            => new(this) { Package = package };
    }
}
