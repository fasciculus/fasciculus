using Fasciculus.Maui.Support;
using Fasciculus.Steam.Models;
using Fasciculus.Steam.Services;
using Fasciculus.Support;
using Fasciculus.Threading;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Fasciculus.Eve.Assets.Services
{
    public interface ICopyImages
    {
        public Task<FileInfo[]> FilesCopied { get; }
    }

    public class CopyImages : ICopyImages
    {
        private readonly ISteamApplications steamApplications;
        private readonly IAssetsDirectories assetsDirectories;
        private readonly IAssetsProgress progress;

        private FileInfo[]? files = null;
        private readonly TaskSafeMutex filesMutex = new();

        public Task<FileInfo[]> FilesCopied => GetFilesCopiedAsync();

        public CopyImages(ISteamApplications steamApplications, IAssetsDirectories assetsDirectories, IAssetsProgress progress)
        {
            this.steamApplications = steamApplications;
            this.assetsDirectories = assetsDirectories;
            this.progress = progress;
        }

        private Task<FileInfo[]> GetFilesCopiedAsync()
        {
            using Locker locker = Locker.Lock(filesMutex);

            return Tasks.LongRunning(() => GetFilesCopied());
        }

        private FileInfo[] GetFilesCopied()
        {
            if (files is null)
            {
                Tuple<FileInfo, FileInfo>[] entries = ParseIndex();

                progress.CopyImagesProgress.Begin(entries.Length);
                files = entries.AsParallel().Select(Copy).ToArray();
                progress.CopyImagesProgress.End();
            }

            return files;
        }

        private FileInfo Copy(Tuple<FileInfo, FileInfo> entry)
        {
            FileInfo source = entry.Item1;
            FileInfo destination = entry.Item2;

            if (source.IsNewerThan(destination))
            {
                destination.Directory?.CreateIfNotExists();
                destination.DeleteIfExists();
                source.CopyTo(destination.FullName);
            }

            progress.CopyImagesProgress.Report(1);

            return new(destination.FullName);
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
                .AsParallel()
                .Select(v => CreateIndexEntry(v, resFiles, steamImages))
                .NotNull()
                .ToArray();
        }

        private static Tuple<FileInfo, FileInfo>? CreateIndexEntry(Tuple<string, string> value, DirectoryInfo resFiles,
            DirectoryInfo steamImages)
        {
            FileInfo source = resFiles.File(value.Item1);

            return source.Exists ? new(source, steamImages.File(value.Item2)) : null;
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
        public Task<List<FileInfo>> FilesCreated { get; }
    }

    public class CreateImages : ICreateImages
    {
        private static readonly Dictionary<string, string> paths = new()
        {
            { "industry.png", "ui/texture/windowicons/industry.png"},
            { "info.png", "ui/texture/windowicons/info.png"},
            { "map.png", "ui/texture/windowicons/map.png"},
            { "market.png", "ui/texture/windowicons/market.png"},
            { "planets.png", "ui/texture/windowicons/planets.png"},
            { "skills.png", "ui/texture/windowicons/skills.png"},
            { "warning.png", "ui/texture/windowicons/warning.png"},
        };

        private readonly ICopyImages copyImages;
        private readonly IAssetsDirectories assetsDirectories;
        private readonly IWriteResource writeResource;
        private readonly IAssetsProgress progress;

        private List<FileInfo>? filesCreated = null;
        private readonly TaskSafeMutex filesCreatedMutex = new();

        public Task<List<FileInfo>> FilesCreated => GetFilesCreatedAsync();

        public CreateImages(ICopyImages copyImages, IAssetsDirectories assetsDirectories, IWriteResource writeResource,
            IAssetsProgress progress)
        {
            this.copyImages = copyImages;
            this.assetsDirectories = assetsDirectories;
            this.writeResource = writeResource;
            this.progress = progress;
        }

        private Task<List<FileInfo>> GetFilesCreatedAsync()
        {
            using Locker locker = Locker.Lock(filesCreatedMutex);

            return Tasks.LongRunning(() => GetFilesCreated());
        }

        private List<FileInfo> GetFilesCreated()
        {
            if (filesCreated is null)
            {
                Tasks.Wait(copyImages.FilesCopied);

                progress.CreateImagesProgress.Report(WorkState.Working);
                filesCreated = paths.Select(CreateImage).NotNull().ToList();
                progress.CreateImagesProgress.Report(WorkState.Done);
            }

            return filesCreated;
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
