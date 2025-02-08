using Markdig.Parsers;
using Markdig.Syntax;
using System.Xml.Linq;

namespace Fasciculus.Markdown.Svg
{
    public class SvgBlock : LeafBlock
    {
        public XElement Svg { get; }

        public SvgBlock(BlockParser? parser, XElement svg)
            : base(parser)
        {
            Svg = svg;
        }
    }
}
