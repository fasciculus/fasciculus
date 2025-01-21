using ColorCode;
using Markdig.Helpers;
using Markdig.Parsers;
using Markdig.Renderers;
using Markdig.Renderers.Html;
using Markdig.Syntax;
using System.Linq;

namespace Fasciculus.Markdown.ColorCode
{
    /// <summary>
    /// Replacement for <see cref="HtmlObjectRenderer{CodeBlock}"/>.
    /// </summary>
    public class ColorCodeRenderer : HtmlObjectRenderer<CodeBlock>
    {
        private readonly CodeBlockRenderer fallbackRenderer;
        private readonly HtmlClassFormatter classFormatter = new();

        /// <summary>
        /// Initializes a new renderer with the given <paramref name="fallbackRenderer"/> as fallback.
        /// </summary>
        /// <param name="fallbackRenderer"></param>
        public ColorCodeRenderer(CodeBlockRenderer fallbackRenderer)
        {
            this.fallbackRenderer = fallbackRenderer;
        }

        /// <summary>
        /// Writes the specified Markdown <paramref name="codeBlock"/> to the given <paramref name="renderer"/>.
        /// </summary>
        protected override void Write(HtmlRenderer renderer, CodeBlock codeBlock)
        {
            FencedCodeBlock? fencedCodeBlock = codeBlock as FencedCodeBlock;
            FencedCodeBlockParser? fencedCodeBlockParser = codeBlock.Parser as FencedCodeBlockParser;
            ILanguage? language = ExtractLanguage(fencedCodeBlock, fencedCodeBlockParser);

            if (language is null)
            {
                fallbackRenderer.Write(renderer, codeBlock);

                return;
            }

            string code = ExtractCode(codeBlock);
            string html = classFormatter.GetHtmlString(code, language) ?? string.Empty;

            renderer.Write(html);
        }

        private static string ExtractCode(CodeBlock codeBlock)
        {
            StringLine[] lines = codeBlock.Lines.Lines ?? [];

            return string.Join("\r\n", lines
                .Select(line => line.Slice)
                .Where(slice => slice.Text is not null)
                .Select(slice => slice.Text.Substring(slice.Start, slice.Length)));
        }

        private static ILanguage? ExtractLanguage(FencedCodeBlock? fencedCodeBlock, FencedCodeBlockParser? fencedCodeBlockParser)
        {
            string? info = fencedCodeBlock?.Info;
            string? infoPrefix = fencedCodeBlockParser?.InfoPrefix;
            string? languageId = info is null || infoPrefix is null ? null : info.Replace(infoPrefix, string.Empty);

            return string.IsNullOrWhiteSpace(languageId) ? null : Languages.FindById(languageId);
        }
    }
}
