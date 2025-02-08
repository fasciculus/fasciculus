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

        public string GetEntry(string key)
        {
            return GetString($"Blog/Entry/{key}");
        }
    }
}
