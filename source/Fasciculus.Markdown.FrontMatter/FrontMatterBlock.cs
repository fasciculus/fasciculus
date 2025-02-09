using Fasciculus.Yaml;
using Markdig.Parsers;
using Markdig.Syntax;
using System.Collections.Generic;

namespace Fasciculus.Markdown.FrontMatter
{
    public class FrontMatterBlock : LeafBlock
    {
        public string[] Keys { get; }

        public YDictionary Entries { get; set; } = YDictionary.Empty;

        public FrontMatterBlock(BlockParser? parser, IEnumerable<string> keys)
            : base(parser)
        {
            Keys = [.. keys];
        }
    }
}
