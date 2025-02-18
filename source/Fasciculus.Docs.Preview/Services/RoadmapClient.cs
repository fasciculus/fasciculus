using System.IO;
using System.Net.Http;

namespace Fasciculus.Docs.Preview.Services
{
    public class RoadmapClient : ContentClient
    {
        public RoadmapClient(HttpClient httpClient)
            : base(httpClient) { }

        public string[] GetKeys()
        {
            string text = GetString("Roadmap/Keys");

            return text.Split(',');
        }

        public FileInfo GetFile(string key)
        {
            return new(GetString($"Roadmap/File/{key}"));
        }
    }
}
