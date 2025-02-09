using System.IO;
using System.Net.Http;

namespace Fasciculus.Docs.Preview.Services
{
    public class SpecificationClient : ContentClient
    {
        public SpecificationClient(HttpClient httpClient)
            : base(httpClient) { }

        public string[] GetKeys()
        {
            string text = GetString("Specifications/Keys");

            return text.Split(',');
        }

        public FileInfo GetFile(string key)
        {
            return new(GetString($"Specifications/File/{key}"));
        }

    }
}
