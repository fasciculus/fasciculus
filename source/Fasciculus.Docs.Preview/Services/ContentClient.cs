using Fasciculus.Threading;
using System;
using System.Net.Http;

namespace Fasciculus.Docs.Preview.Services
{
    public class ContentClient
    {
        private readonly HttpClient httpClient;

        public ContentClient(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public DateTime GetVersion()
        {
            string text = GetString("Version");

            return DateTime.FromBinary(long.Parse(text));
        }

        protected string GetString(string uri)
        {
            return Tasks.Wait(httpClient.GetStringAsync($"http://localhost:5256/{uri}"));
        }
    }
}
