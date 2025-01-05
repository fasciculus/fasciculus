using Fasciculus.Collections;
using Fasciculus.Reflection;
using Fasciculus.Threading;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Builder;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Fasciculus.Site.Services
{
    public class Generator
    {
        private readonly RequestDelegate requestDelegate;
        private readonly IFeatureCollection serverFeatures;
        private readonly IHttpContextFactory httpContextFactory;
        private readonly Documents documents;
        private readonly Writer writer;

        public Generator(IEnumerable<IHostedService> hostedServices, IEnumerable<IStartupFilter> startupFilters,
            IServer server, IApplicationBuilderFactory applicationBuilderFactory, IHttpContextFactory httpContextFactory,
            Documents documents, Writer writer)
        {
            requestDelegate = BuildRequestDelegate(hostedServices, startupFilters, server, applicationBuilderFactory);
            serverFeatures = server.Features;

            this.httpContextFactory = httpContextFactory;
            this.documents = documents;
            this.writer = writer;
        }

        public void Run()
        {
            documents.Apply(Generate);
        }

        private void Generate(string document)
        {
            using MemoryStream responseBody = new();
            FeatureCollection features = CreateFeatures(document, responseBody);
            HttpContext httpContext = httpContextFactory.Create(features);

            Tasks.Wait(requestDelegate(httpContext));

            writer.Write(document, httpContext.Response.ContentType ?? string.Empty, responseBody.ToArray());
        }

        private FeatureCollection CreateFeatures(string document, Stream responseBody)
        {
            HttpRequestFeature httpRequestFeature = new()
            {
                Protocol = "HTTP/1.1",
                Method = "GET",
                Path = document,
                RawTarget = document
            };

            HttpResponseFeature httpResponseFeature = new()
            {
                Body = responseBody
            };

            StreamResponseBodyFeature httpResponseBodyFeature = new(responseBody);

            FeatureCollection features = new(serverFeatures);

            features.Set<IHttpRequestFeature>(httpRequestFeature);
            features.Set<IHttpResponseFeature>(httpResponseFeature);
            features.Set<IHttpResponseBodyFeature>(httpResponseBodyFeature);

            return features;
        }

        private static RequestDelegate BuildRequestDelegate(IEnumerable<IHostedService> hostedServices,
            IEnumerable<IStartupFilter> startupFilters, IServer server, IApplicationBuilderFactory applicationBuilderFactory)
        {
            IHostedService webHost = hostedServices.OfType("Microsoft.AspNetCore.Hosting.GenericWebHostService").Single();
            object options = webHost.GetRequiredProperty<object>("Options");
            Action<IApplicationBuilder> configure = options.GetRequiredProperty<Action<IApplicationBuilder>>("ConfigureApplication");
            IApplicationBuilder applicationBuilder = applicationBuilderFactory.CreateBuilder(server.Features);

            foreach (var startupFilter in Enumerable.Reverse(startupFilters))
            {
                configure = startupFilter.Configure(configure);
            }

            configure(applicationBuilder);

            return applicationBuilder.Build();
        }
    }
}
