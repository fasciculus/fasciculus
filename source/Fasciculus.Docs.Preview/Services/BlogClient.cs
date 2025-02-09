using System.IO;
using System.Net.Http;

namespace Fasciculus.Docs.Preview.Services
{
    public class BlogClient : ContentClient
    {
        public BlogClient(HttpClient httpClient)
            : base(httpClient) { }

        public string[] GetKeys()
        {
            string text = GetString("Blog/Keys");

            return text.Split(',');
        }

        public FileInfo GetFile(string key)
        {
            return new(GetString($"Blog/File/{key}"));
        }
    }
}
