using Markdig.Parsers;
using System.Linq;
using System.Text.RegularExpressions;

namespace Fasciculus.Markdown.FrontMatter
{
    public class FrontMatterParser : BlockParser
    {
#pragma warning disable SYSLIB1045 // Convert to 'GeneratedRegexAttribute'.
        public static readonly Regex FrontMatterRegex = new(@"^!frontmatter\{([^}]+)\}\s*$");
#pragma warning restore SYSLIB1045 // Convert to 'GeneratedRegexAttribute'.

        public FrontMatterParser()
        {
            OpeningCharacters = ['!'];
        }

        public override BlockState TryOpen(BlockProcessor processor)
        {
            if (processor.IsCodeIndent)
            {
                return BlockState.None;
            }

            Match match = FrontMatterRegex.Match(processor.Line.ToString());

            if (match.Success)
            {
                string[] keys = [.. match.Groups[1].Value.Split(',').Select(s => s.Trim())];
                FrontMatterBlock block = new(this, keys);

                processor.NewBlocks.Push(block);
                processor.Consume(match.Length);

                return BlockState.Break;
            }

            return BlockState.None;
        }
    }
}
