using Fasciculus.Eve.Models;
using Fasciculus.Threading;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Fasciculus.Eve.Assets.Services
{
    public interface IConvertData
    {
        public Task<EveData> ConvertAsync();
    }

    public class ConvertData : IConvertData
    {
        private readonly IDownloadSde downloadSde;
        private readonly IAssetsProgress progress;

        private EveData? result;
        private readonly TaskSafeMutex mutex = new();

        public ConvertData(IDownloadSde downloadSde, IAssetsProgress progress)
        {
            this.downloadSde = downloadSde;
            this.progress = progress;
        }

        public async Task<EveData> ConvertAsync()
        {
            using Locker locker = Locker.Lock(mutex);

            await Task.Yield();

            if (result is null)
            {
                progress.ConvertData.Report(PendingToDone.Working);

                DateTime version = downloadSde.Download().LastWriteTimeUtc;
                EveData.Data data = new(version);

                result = new(data);

                progress.ConvertData.Report(PendingToDone.Done);
            }

            return result;
        }
    }

    public static class ConvertDataServices
    {
        public static IServiceCollection AddConvertData(this IServiceCollection services)
        {
            services.AddSdeZip();
            services.AddAssetsProgress();

            services.TryAddSingleton<IConvertData, ConvertData>();

            return services;
        }
    }
}
