using NuGet.Protocol.Core.Types;

namespace Fasciculus.NuGet.Protocol
{
    public class NuGetRepository
    {
        public SourceRepository Repository { get; }

        public NuGetResources Resources { get; }

        public NuGetRepository(SourceRepository repository)
        {
            Repository = repository;
            Resources = new([repository]);
        }
    }
}
