using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Fasciculus.Net
{
    /// <summary>
    /// Result returned by <see cref="Downloader"/>.
    /// </summary>
    public class DownloaderResult
    {
        /// <summary>
        /// The downloaded file.
        /// </summary>
        public FileInfo DownloadedFile { get; }

        /// <summary>
        /// Whether the downloaded wasn't modified.
        /// </summary>
        public bool NotModified { get; }

        /// <summary>
        /// Initializes a new downloader result.
        /// </summary>
        public DownloaderResult(FileInfo downloadedFile, bool notModified)
        {
            DownloadedFile = downloadedFile;
            NotModified = notModified;
        }
    }

    /// <summary>
    /// Downloader.
    /// </summary>
    public static class Downloader
    {
        /// <summary>
        /// Downloads the given <paramref name="uri"/> into the given <paramref name="destination"/> if the file doesn't exist
        /// or is older than the server resource.
        /// </summary>
        public static async Task<DownloaderResult> DownloadAsync(HttpClient httpClient, Uri uri, FileInfo destination)
        {
            HttpRequestMessage httpRequest = new(HttpMethod.Get, uri);
            bool notModified = false;

            if (File.Exists(destination.FullName))
            {
                httpRequest.Headers.IfModifiedSince = destination.LastWriteTimeUtc;
            }

            HttpResponseMessage httpResponse = await httpClient.SendAsync(httpRequest);

            if (httpResponse.StatusCode == HttpStatusCode.NotModified)
            {
                notModified = true;
            }
            else
            {
                httpResponse.EnsureSuccessStatusCode();

                byte[] bytes = await httpResponse.Content.ReadAsByteArrayAsync();

                destination.DeleteIfExists();
                destination.WriteAllBytes(bytes);
                destination = new(destination.FullName);
            }

            return new(destination, notModified);
        }
    }
}
