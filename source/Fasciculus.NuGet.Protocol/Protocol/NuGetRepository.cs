using Fasciculus.NuGet.Configuration;
using Fasciculus.Threading;
using NuGet.Protocol.Core.Types;
using System.Threading.Tasks;

namespace Fasciculus.NuGet.Protocol
{
    public class NuGetRepository
    {
        public static AsyncLazy<NuGetRepository> Local { get; } = new(GetLocal);

        private static async Task<NuGetRepository> GetLocal()
        {
            NuGetSource source = await NuGetSource.Local;

            return source.GetRepository();
        }

        public SourceRepository Repository { get; }

        public NuGetResources Resources { get; }

        public NuGetRepository(SourceRepository repository)
        {
            Repository = repository;
            Resources = new([repository]);
        }
    }
}
