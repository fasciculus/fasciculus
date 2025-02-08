using Fasciculus.Blog.Preview.Models;
using Fasciculus.Markdown;
using Markdig;
using System;
using System.Net.Http;

namespace Fasciculus.Blog.Preview.Services
{
    public class Entries : Client
    {
        private readonly MarkdownPipeline pipeline;

        public Entries(HttpClient client, Graphics graphics)
            : base(client)
        {
            pipeline = new MarkdownPipelineBuilder()
                .UseAlertBlocks()
                .UseMathematics()
                .UsePipeTables()
                .UseBootstrap()
                .UseSvg(graphics)
                .Build();
        }

        public string[] GetKeys()
        {
            string text = GetString("Keys");

            return text.Split(',');
        }

        public Entry GetEntry(string key)
        {
            string markdown = GetString($"Entry/{key}");
            string html = Markdig.Markdown.ToHtml(markdown, pipeline);

            return new()
            {
                Title = key,
                Content = html,
            };
        }

        public DateTime GetVersion()
        {
            string text = GetString("Version");

            return DateTime.FromBinary(long.Parse(text));
        }
    }
}
