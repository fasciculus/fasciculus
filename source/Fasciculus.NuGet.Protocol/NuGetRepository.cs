using NuGet.Protocol.Core.Types;

namespace Fasciculus.NuGet.Protocol
{
    public class NuGetRepository
    {
        public SourceRepository Repository { get; }

        public NuGetRepository(SourceRepository repository)
        {
            Repository = repository;
        }
    }
}
