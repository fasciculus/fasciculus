using Fasciculus.Steam.Models;
using Fasciculus.Steam.Services;
using Fasciculus.Support;
using Fasciculus.Threading;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Fasciculus.Eve.Assets.Services
{
    public interface ICopyImages
    {
        public Task CopyAsync();
    }

    public class CopyImages : ICopyImages
    {
        private readonly ISteamApplications steamApplications;
        private readonly IAssetsDirectories assetsDirectories;
        private readonly IAssetsProgress progress;

        private bool result = false;
        private readonly TaskSafeMutex mutex = new();

        public CopyImages(ISteamApplications steamApplications, IAssetsDirectories assetsDirectories, IAssetsProgress progress)
        {
            this.steamApplications = steamApplications;
            this.assetsDirectories = assetsDirectories;
            this.progress = progress;
        }

        public async Task CopyAsync()
        {
            using Locker locker = Locker.Lock(mutex);

            await Task.Yield();

            if (!result)
            {
                Tuple<FileInfo, FileInfo>[] entries = ParseIndex();

                progress.CopyImages.Begin(entries.Length);

                entries.Apply(Copy);

                progress.CopyImages.End();

                result = true;
            }
        }

        private void Copy(Tuple<FileInfo, FileInfo> entry)
        {
            FileInfo source = entry.Item1;
            FileInfo destination = entry.Item2;

            if (source.IsNewerThan(destination))
            {
                destination.Directory?.CreateIfNotExists();
                destination.DeleteIfExists();
                source.CopyTo(destination.FullName);
            }

            progress.CopyImages.Report(1);
        }

        private Tuple<FileInfo, FileInfo>[] ParseIndex()
        {
            SteamApplication eveOnline = Cond.NotNull(steamApplications["EVE Online"]);
            DirectoryInfo sharedCache = eveOnline.InstallationDirectory!.Combine("Shared Cache");
            FileInfo indexFile = sharedCache.Combine("tq").File("resfileindex.txt");
            DirectoryInfo resFiles = sharedCache.Combine("ResFiles");
            DirectoryInfo steamImages = assetsDirectories.SteamImages;

            return indexFile
                .ReadAllLines()
                .Select(ParseIndexLine)
                .NotNull()
                .Select(v => CreateIndexEntry(v, resFiles, steamImages))
                .NotNull()
                .ToArray();
        }

        private static Tuple<FileInfo, FileInfo>? CreateIndexEntry(Tuple<string, string> value, DirectoryInfo resFiles,
            DirectoryInfo steamImages)
        {
            FileInfo source = resFiles.File(value.Item1);
            FileInfo destination = steamImages.File(value.Item2);

            return source.Exists ? new(source, destination) : null;
        }

        private static Tuple<string, string>? ParseIndexLine(string line)
        {
            if (line.Length == 0) return null;

            string[] parts = line.Split(',');
            bool valid = parts.Length > 1;

            valid = valid && parts[0].StartsWith("res:/");
            valid = valid && parts[0].EndsWith(".png");

            return valid ? new(parts[1], parts[0].Substring(5)) : null;
        }
    }

    public interface ICreateImages
    {
        public Task<List<FileInfo>> CreateAsync();
    }

    public class CreateImages : ICreateImages
    {
        private static readonly Dictionary<string, string> paths = new()
        {
            { "industry.png", "ui/texture/windowicons/industry.png"},
            { "info.png", "ui/texture/windowicons/info.png"},
            { "map.png", "ui/texture/windowicons/map.png"},
            { "market.png", "ui/texture/windowicons/market.png"},
        };

        private readonly ICopyImages copyImages;
        private readonly IAssetsDirectories assetsDirectories;
        private readonly IWriteResource writeResource;
        private readonly IAssetsProgress progress;

        private List<FileInfo>? result;
        private TaskSafeMutex mutex = new();

        public CreateImages(ICopyImages copyImages, IAssetsDirectories assetsDirectories, IWriteResource writeResource,
            IAssetsProgress progress)
        {
            this.copyImages = copyImages;
            this.assetsDirectories = assetsDirectories;
            this.writeResource = writeResource;
            this.progress = progress;
        }

        public async Task<List<FileInfo>> CreateAsync()
        {
            using Locker locker = Locker.Lock(mutex);

            await Task.Yield();

            if (result is null)
            {
                await copyImages.CopyAsync();

                progress.CreateImages.Report(PendingToDone.Working);
                result = paths.Select(CreateImage).NotNull().ToList();
                progress.CreateImages.Report(PendingToDone.Done);
            }

            await copyImages.CopyAsync();

            return result;
        }

        private FileInfo? CreateImage(KeyValuePair<string, string> kvp)
        {
            FileInfo source = assetsDirectories.SteamImages.File(kvp.Value);
            FileInfo destination = assetsDirectories.Images.File(kvp.Key);

            return writeResource.Copy(source, destination, false) ? destination : null;
        }
    }

    public static class ImageServices
    {
        public static IServiceCollection AddImages(this IServiceCollection services)
        {
            services.AddSteam();
            services.AddAssetsDirectories();
            services.AddAssetsProgress();
            services.AddWriteResource();

            services.TryAddSingleton<ICopyImages, CopyImages>();
            services.TryAddSingleton<ICreateImages, CreateImages>();

            return services;
        }
    }
}
