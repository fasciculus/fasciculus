using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace Fasciculus.Eve.Operations
{
    public static class DownloadSdeZip
    {
        public static readonly Uri SdeZipUri = new("https://eve-static-data-export.s3-eu-west-1.amazonaws.com/tranquility/sde.zip");

        public static async Task Execute()
        {
            using HttpClient httpClient = new();

            if (await IsDownloadRequired(httpClient))
            {
                await Download(httpClient);
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
