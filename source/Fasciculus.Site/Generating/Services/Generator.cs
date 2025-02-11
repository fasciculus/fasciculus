using Fasciculus.Collections;
using Fasciculus.Web.Extensions;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace Fasciculus.Site.Generating.Services
{
    public class Generator
    {
        private readonly RequestInvoker invoker;

        private readonly GeneratorDocuments documents;
        private readonly GeneratorWriter writer;

        public Generator(RequestInvoker invoker, GeneratorDocuments documents, GeneratorWriter writer)
        {
            this.invoker = invoker;
            this.documents = documents;
            this.writer = writer;
        }

        public void Run()
        {
            writer.Reset();

            documents.Apply(Generate);
        }

        private void Generate(string document)
        {
            using MemoryStream responseBody = new();
            HttpResponse response = invoker.Invoke(document, responseBody);

            writer.Write(document, response.ContentType ?? string.Empty, responseBody.ToArray());
        }
    }
}
