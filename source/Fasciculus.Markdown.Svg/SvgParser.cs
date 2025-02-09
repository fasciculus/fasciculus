using Markdig.Parsers;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace Fasciculus.Markdown.Svg
{
    public class SvgParser : BlockParser
    {
#pragma warning disable SYSLIB1045 // Convert to 'GeneratedRegexAttribute'.
        public static readonly Regex SvgRegex = new(@"^!svg\{([^,]+),([^,]+),([^}]+)\}\s*$");
#pragma warning restore SYSLIB1045 // Convert to 'GeneratedRegexAttribute'.

        private readonly ISvgMappings mappings;

        public SvgParser(ISvgMappings mappings)
        {
            this.mappings = mappings;

            OpeningCharacters = ['!'];
        }

        public override BlockState TryOpen(BlockProcessor processor)
        {
            if (processor.IsCodeIndent)
            {
                return BlockState.None;
            }

            Match match = SvgRegex.Match(processor.Line.ToString());

            if (match.Success)
            {
                string key = match.Groups[1].Value.Trim();
                XElement? svg = mappings.GetSvg(key);

                if (svg is not null)
                {
                    svg.SetAttributeValue("width", match.Groups[2].Value.Trim());
                    svg.SetAttributeValue("height", match.Groups[3].Value.Trim());

                    processor.NewBlocks.Push(new SvgBlock(this, svg));
                    processor.Consume(match.Length);

                    return BlockState.Break;
                }
            }

            return BlockState.None;
        }
    }
}
