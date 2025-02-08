using Markdig.Parsers;

namespace Fasciculus.Markdown
{
    public static class BlockProcessorExtensions
    {
        public static void Consume(this BlockProcessor processor, int count)
        {
            for (int i = 0; i < count; ++i)
            {
                processor.NextChar();
            }
        }
    }
}
