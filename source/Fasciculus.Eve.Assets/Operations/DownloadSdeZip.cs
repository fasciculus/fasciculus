using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace Fasciculus.Eve.Operations
{
    public static class DownloadSdeZip
    {
        public static readonly Uri SdeZipUri = new("https://eve-static-data-export.s3-eu-west-1.amazonaws.com/tranquility/sde.zip");

        public static async Task Execute(IProgress<string> progress)
        {
            using HttpClient httpClient = new();

            if (await IsDownloadRequired(httpClient))
            {
                progress.Report("downloading sde.zip");
                await Download(httpClient);
                progress.Report("downloading sde.zip done");
            }
            else
            {
                progress.Report("sde.zip is up to date");
            }
        }

        private static async Task<bool> IsDownloadRequired(HttpClient httpClient)
        {
            FileInfo file = EveAssetsFiles.SdeZipFile;

            if (file.Exists)
            {
                using HttpRequestMessage request = new(HttpMethod.Head, SdeZipUri);
                using HttpResponseMessage response = await httpClient.SendAsync(request);

                response.EnsureSuccessStatusCode();

                long contentLength = response.Content.Headers.ContentLength ?? 0;

                return contentLength != file.Length;
            }
            else
            {
                return true;
            }
        }

        private static async Task Download(HttpClient httpClient)
        {
            byte[] bytes = await httpClient.GetByteArrayAsync(SdeZipUri);

            EveAssetsFiles.SdeZipFile.WriteAllBytes(bytes);
        }
    }
}
