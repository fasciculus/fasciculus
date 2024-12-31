using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Builder;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Fasciculus.GitHub
{
    public static class Generator
    {
        public static void Generate(WebApplication app)
        {
            IServer server = app.Services.GetRequiredService<IServer>();
            IHttpContextFactory httpContextFactory = app.Services.GetRequiredService<IHttpContextFactory>();
            IFeatureCollection serverFeatures = server.Features;

            IActionDescriptorCollectionProvider actionDescriptorCollectionProvider = app.Services.GetRequiredService<IActionDescriptorCollectionProvider>();
            IReadOnlyList<ActionDescriptor> actionDescriptors = actionDescriptorCollectionProvider.ActionDescriptors.Items;

            IApplicationBuilderFactory applicationBuilderFactory = app.Services.GetRequiredService<IApplicationBuilderFactory>();
            IApplicationBuilder applicationBuilder = applicationBuilderFactory.CreateBuilder(serverFeatures);

            applicationBuilder.ApplicationServices = app.Services;

            var startupFilters = app.Services.GetRequiredService<IEnumerable<IStartupFilter>>();

            foreach (var startupFilter in Enumerable.Reverse(startupFilters))
            {
                Debug.WriteLine(startupFilter.GetType().FullName);
            }

            IEnumerable<IHostedService> hostedServices = app.Services.GetRequiredService<IEnumerable<IHostedService>>();
            IHostedService hostedService = hostedServices.First(x => "Microsoft.AspNetCore.Hosting.GenericWebHostService" == x.GetType().FullName);
            Type hostedServiceType = hostedService.GetType();
            PropertyInfo optionsPropertyInfo = hostedServiceType.GetProperty("Options")!;
            object optionsObject = optionsPropertyInfo.GetValue(hostedService)!;
            Type optionsType = optionsObject.GetType();
            PropertyInfo configureApplicationPropertyInfo = optionsType.GetProperty("ConfigureApplication")!;
            object configureApplicationObject = configureApplicationPropertyInfo.GetValue(optionsObject)!;

            Action<IApplicationBuilder> configure = (Action<IApplicationBuilder>)configureApplicationObject;

            foreach (var startupFilter in Enumerable.Reverse(startupFilters))
            {
                configure = startupFilter.Configure(configure);
            }

            configure(applicationBuilder);

            RequestDelegate requestDelegate = applicationBuilder.Build();

            HttpRequestFeature httpRequestFeature = new()
            {
                Protocol = "HTTP/1.1",
                Method = "GET",
                Path = "/",
                RawTarget = "/"
            };

            using MemoryStream responseBody = new();

            HttpResponseFeature httpResponseFeature = new()
            {
                Body = responseBody
            };

            StreamResponseBodyFeature httpResponseBodyFeature = new(responseBody);

            FeatureCollection features = new(serverFeatures);

            features.Set<IHttpRequestFeature>(httpRequestFeature);
            features.Set<IHttpResponseFeature>(httpResponseFeature);
            features.Set<IHttpResponseBodyFeature>(httpResponseBodyFeature);

            HttpContext httpContext = new DefaultHttpContext(features);

            requestDelegate(httpContext).GetAwaiter().GetResult();
        }
    }
}
