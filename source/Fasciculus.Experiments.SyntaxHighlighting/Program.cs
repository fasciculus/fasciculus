using Fasciculus.Collections;
using Fasciculus.IO;
using Fasciculus.IO.Resources;
using Markdig;
using Markdig.Syntax;
using Microsoft.ClearScript.V8;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace Fasciculus.Experiments.SyntaxHighlighting
{
    public static class Program
    {
        private static readonly string[] DocumentNames = ["Document1"];

        public static void Main(string[] args)
        {
            Log($"WorkingDirectory = {SpecialDirectories.WorkingDirectory}");

            DocumentNames.Apply(ProcessDocument);
        }

        private static void ProcessDocument(string documentName)
        {
            string markdown = EmbeddedResources.Find(documentName).Read(ReadDocument, false);

            MarkdownPipeline markdownPipeline = new MarkdownPipelineBuilder().Build();
            MarkdownDocument markdownDocument = Markdown.Parse(markdown, markdownPipeline);
            string html = markdownDocument.ToHtml(markdownPipeline);

            using V8ScriptEngine engine = new();

            engine.AddHostType("Console", typeof(Console));
            engine.Execute("Console.WriteLine('{0} is an interesting number.', Math.PI)");
        }

        private static string ReadDocument(Stream stream)
        {
            byte[] buffer = new byte[stream.Length];

            stream.ReadExactly(buffer);

            return Encoding.UTF8.GetString(buffer);
        }

        private static void Log(object? obj)
        {
            string message = obj?.ToString() ?? string.Empty;

            Console.WriteLine(message);
            Debug.WriteLine(message);
        }
    }
}
