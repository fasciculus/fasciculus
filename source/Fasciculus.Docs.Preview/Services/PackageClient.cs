using System.IO;
using System.Net.Http;

namespace Fasciculus.Docs.Preview.Services
{
    public class PackageClient : ContentClient
    {
        public PackageClient(HttpClient httpClient)
            : base(httpClient) { }

        public string[] GetKeys()
        {
            string text = GetString("Packages/Keys");

            return text.Split(',');
        }

        public FileInfo GetFile(string key)
        {
            return new(GetString($"Packages/File/{key}"));
        }
    }
}
