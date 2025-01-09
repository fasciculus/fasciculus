using Fasciculus.CodeAnalysis.Frameworking;

namespace Fasciculus.CodeAnalysis.Compilers
{
    public class CompilerContext
    {
        public TargetFramework Framework { get; private set; }

        public TargetFrameworks Frameworks => new(Framework);

        public CompilerContext(TargetFramework framework)
        {
            Framework = framework;
        }

        private CompilerContext(CompilerContext other)
        {
            Framework = other.Framework;
        }

        public CompilerContext WithFramework(TargetFramework framework)
        {
            return new(this)
            {
                Framework = framework,
            };
        }
    }
}
