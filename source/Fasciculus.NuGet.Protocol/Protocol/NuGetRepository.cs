using Fasciculus.NuGet.Configuration;
using Fasciculus.Threading;
using NuGet.Protocol.Core.Types;
using System.Threading.Tasks;

namespace Fasciculus.NuGet.Protocol
{
    public class NuGetRepository
    {
        public static AsyncLazy<NuGetRepository> Global { get; } = new(GetGlobal);

        private static async Task<NuGetRepository> GetGlobal()
        {
            NuGetSource source = await NuGetSource.Global;

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
