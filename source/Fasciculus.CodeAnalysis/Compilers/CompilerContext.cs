using Fasciculus.CodeAnalysis.Frameworking;
using System.IO;

namespace Fasciculus.CodeAnalysis.Compilers
{
    public class CompilerContext
    {
        public TargetFramework Framework { get; private set; }

        public TargetFrameworks Frameworks => new(Framework);

        public DirectoryInfo? ProjectDirectory { get; private set; }

        public string Package { get; private set; } = string.Empty;

        public CodeAnalyzerDebuggers Debuggers { get; private set; } = new();

        public CompilerContext(TargetFramework framework)
        {
            Framework = framework;
        }

        private CompilerContext(CompilerContext other)
        {
            Framework = other.Framework;
            ProjectDirectory = other.ProjectDirectory;
            Package = other.Package;
            Debuggers = other.Debuggers;
        }

        public static CompilerContext Create(TargetFramework framework)
            => new(framework);

        public CompilerContext WithFramework(TargetFramework framework)
            => new(this) { Framework = framework };

        public CompilerContext WithProjectDirectory(DirectoryInfo? projectDirectory)
            => new(this) { ProjectDirectory = projectDirectory };

        public CompilerContext WithPackage(string package)
            => new(this) { Package = package };

        public CompilerContext WithDebuggers(CodeAnalyzerDebuggers debuggers)
            => new(this) { Debuggers = debuggers };
    }
}
