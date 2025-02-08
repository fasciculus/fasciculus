using Fasciculus.Threading;
using System.Net.Http;

namespace Fasciculus.Blog.Preview.Services
{
    public class Client
    {
        private readonly HttpClient client;

        public Client(HttpClient client)
        {
            this.client = client;
        }

        protected string GetString(string uri)
        {
            return Tasks.Wait(client.GetStringAsync($"http://localhost:5255/{uri}"));
        }
    }
}
