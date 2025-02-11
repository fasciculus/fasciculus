using System.IO;
using System.Net.Http;

namespace Fasciculus.Docs.Preview.Services
{
    public class NamespaceClient : ContentClient
    {
        public NamespaceClient(HttpClient httpClient)
            : base(httpClient) { }

        public string[] GetKeys()
        {
            string text = GetString("Namespaces/Keys");

            return text.Split(',');
        }

        public FileInfo GetFile(string key)
        {
            return new(GetString($"Namespaces/File/{key}"));
        }
    }
}
