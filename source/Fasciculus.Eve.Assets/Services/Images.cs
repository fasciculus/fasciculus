using Fasciculus.Steam.Models;
using Fasciculus.Steam.Services;
using Fasciculus.Support;
using Fasciculus.Utilities;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Fasciculus.Eve.Assets.Services
{
    public interface IImageCopier
    {
        public void Copy();
    }

    public class ImageCopier : IImageCopier
    {
        private readonly ISteamApplications steamApplications;
        private readonly IAssetsDirectories assetsDirectories;
        private readonly ILongProgress progress;

        public ImageCopier(ISteamApplications steamApplications, IAssetsDirectories assetsDirectories,
            [FromKeyedServices(ServiceKeys.ImageCopier)] ILongProgress progress)
        {
            this.steamApplications = steamApplications;
            this.assetsDirectories = assetsDirectories;
            this.progress = progress;
        }

        public void Copy()
        {
            Tuple<FileInfo, FileInfo>[] entries = ParseIndex();

            progress.Start(entries.Length);

            entries.Apply(Copy);

            progress.Done();
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

            progress.Report(1);
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

    public static class ImageServices
    {
        public static IServiceCollection AddImages(this IServiceCollection services)
        {
            services.AddSteam();
            services.AddAssetsDirectories();
            services.AddAssetsProgress();

            services.TryAddSingleton<IImageCopier, ImageCopier>();

            return services;
        }
    }
}
