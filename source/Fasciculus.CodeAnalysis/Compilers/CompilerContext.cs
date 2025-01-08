using Fasciculus.CodeAnalysis.Frameworking;

namespace Fasciculus.CodeAnalysis.Compilers
{
    public class CompilerContext
    {
        public required TargetFramework Framework { get; init; }

        public TargetFrameworks Frameworks => new(Framework);
    }
}
