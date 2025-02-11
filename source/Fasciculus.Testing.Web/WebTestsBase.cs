using Fasciculus.Collections;
using Fasciculus.Web.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;

namespace Fasciculus.Testing.Web
{
    public abstract class WebTestsBase : IDisposable
    {
        private readonly DisposableList<Stream> streams = [];

        private readonly Lazy<WebApplicationBuilder> builder;
        private readonly Lazy<WebApplication> app;
        private readonly Lazy<RequestInvoker> invoker;

        protected WebTestsBase()
        {
            builder = new(CreateBuilder, true);
            app = new(CreateApp, true);
            invoker = new(GetInvoker, true);
        }

        ~WebTestsBase()
        {
            Dispose(false);
        }

        public virtual void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool _)
        {
            streams.Dispose();
        }

        protected HttpResponse Invoke(string uri)
        {
            MemoryStream stream = new MemoryStream();

            streams.Add(stream);

            HttpResponse response = invoker.Value.Invoke(uri, stream);

            stream.Position = 0;

            return response;
        }

        protected abstract void Configure(WebApplicationBuilder builder);

        protected abstract void Configure(WebApplication app);

        private WebApplicationBuilder CreateBuilder()
        {
            WebApplicationBuilder builder = WebApplication.CreateBuilder();

            builder.Services.AddRequestInvoker();

            Configure(builder);

            return builder;
        }

        private WebApplication CreateApp()
        {
            WebApplication app = builder.Value.Build();

            Configure(app);

            return app;
        }

        private RequestInvoker GetInvoker()
        {
            return app.Value.Services.GetRequiredService<RequestInvoker>();
        }
    }
}
