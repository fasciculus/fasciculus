using Fasciculus.Eve.IO;
using System;
using System.IO;
using System.Net.Http;

namespace Fasciculus.Eve.Operations
{
    public static class DownloadSdeZip
    {
        public static readonly Uri SdeZipUri = new("https://eve-static-data-export.s3-eu-west-1.amazonaws.com/tranquility/sde.zip");

        public static void Execute(IProgress<string> progress)
        {
            using HttpClient httpClient = new();

            if (IsDownloadRequired(httpClient, progress))
            {
                progress.Report("downloading sde.zip");
                Download(httpClient);
                progress.Report("downloading sde.zip done");
            }
            else
            {
                progress.Report("sde.zip is up to date");
            }
        }

        private static bool IsDownloadRequired(HttpClient httpClient, IProgress<string> progress)
        {
            FileInfo file = EveAssetsFiles.SdeZipFile;

            progress.Report($"checking sde.zip");

            if (file.Exists)
            {
                return (httpClient.Head(SdeZipUri).ContentLength ?? 0) != file.Length;
            }
            else
            {
                return true;
            }
        }

        private static void Download(HttpClient httpClient)
        {
            byte[] bytes = httpClient.GetByteArray(SdeZipUri);

            EveAssetsFiles.SdeZipFile.WriteAllBytes(bytes);
        }
    }
}
