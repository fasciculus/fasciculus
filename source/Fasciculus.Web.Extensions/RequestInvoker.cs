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

namespace Fasciculus.Web.Extensions
{
    public class RequestInvoker
    {
        private readonly IServer server;
        private readonly IHttpContextFactory httpContextFactory;

        private readonly RequestDelegate requestDelegate;

        public RequestInvoker(IServer server, IHttpContextFactory httpContextFactory, IEnumerable<IHostedService> hostedServices,
            IEnumerable<IStartupFilter> startupFilters, IApplicationBuilderFactory applicationBuilderFactory)
        {
            this.server = server;
            this.httpContextFactory = httpContextFactory;

            requestDelegate = CreateRequestDelegate(hostedServices, startupFilters, server, applicationBuilderFactory);
        }

        public HttpResponse Invoke(string uri, Stream responseBody)
        {
            FeatureCollection features = CreateFeatures(uri, responseBody);
            HttpContext httpContext = httpContextFactory.Create(features);

            requestDelegate(httpContext).WaitFor();

            return httpContext.Response;
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
            FeatureCollection features = new(server.Features);

            features.Set<IHttpRequestFeature>(httpRequestFeature);
            features.Set<IHttpResponseFeature>(httpResponseFeature);
            features.Set<IHttpResponseBodyFeature>(httpResponseBodyFeature);

            return features;
        }

        private static RequestDelegate CreateRequestDelegate(IEnumerable<IHostedService> hostedServices,
            IEnumerable<IStartupFilter> startupFilters, IServer server, IApplicationBuilderFactory applicationBuilderFactory)
        {
            IHostedService webHost = hostedServices.OfType("Microsoft.AspNetCore.Hosting.GenericWebHostService").Single();
            object options = webHost.GetRequiredProperty<object>("Options")!;
            Action<IApplicationBuilder> configure = options.GetRequiredProperty<Action<IApplicationBuilder>>("ConfigureApplication")!;
            IApplicationBuilder applicationBuilder = applicationBuilderFactory.CreateBuilder(server.Features);

            foreach (var startupFilter in startupFilters.Reverse())
            {
                configure = startupFilter.Configure(configure);
            }

            configure(applicationBuilder);

            return applicationBuilder.Build();
        }
    }
}
