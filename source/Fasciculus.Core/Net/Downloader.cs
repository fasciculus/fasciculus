using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Fasciculus.Net
{
    public class DownloaderResult
    {
        public FileInfo DownloadedFile { get; }
        public bool NotModified { get; }

        public DownloaderResult(FileInfo downloadedFile, bool notModified)
        {
            DownloadedFile = downloadedFile;
            NotModified = notModified;
        }
    }

    public interface IDownloader
    {
        public Task<DownloaderResult> DownloadAsync(HttpClient httpClient, Uri uri, FileInfo destination);
    }

    public class Downloader : IDownloader
    {
        public async Task<DownloaderResult> DownloadAsync(HttpClient httpClient, Uri uri, FileInfo destination)
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
