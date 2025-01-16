using Fasciculus.Collections;
using Fasciculus.IO;
using Fasciculus.IO.Resources;
using Markdig;
using Markdig.Syntax;
using System;
using System.Diagnostics;
using System.Text;

namespace Fasciculus.Experiments.SyntaxHighlighting
{
    public static class Program
    {
        private static readonly string[] DocumentNames = ["Document1", "Document2", "Document3"];

        public static void Main(string[] args)
        {
            Log($"WorkingDirectory = {SpecialDirectories.WorkingDirectory}");

            DocumentNames.Apply(ProcessDocument);
        }

        private static void ProcessDocument(string documentName)
        {
            string markdown = EmbeddedResources.Find(documentName).ReadString(false, Encoding.UTF8);

            MarkdownPipelineBuilder markdownPipelineBuilder = new();

            markdownPipelineBuilder.Extensions.Add(new ColorCodeExtension());

            MarkdownPipeline markdownPipeline = markdownPipelineBuilder.Build();

            MarkdownDocument markdownDocument = Markdown.Parse(markdown, markdownPipeline);
            string html = markdownDocument.ToHtml(markdownPipeline);

            Log($"------ {documentName} ------");
            Log(html);
        }

        private static void Log(object? obj)
        {
            string message = obj?.ToString() ?? string.Empty;

            Console.WriteLine(message);
            Debug.WriteLine(message);
        }
    }
}
