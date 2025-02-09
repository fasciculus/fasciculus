namespace Fasciculus.Markdown.FrontMatter
{
    public interface IFrontMapperMappings
    {
        public string? GetLabel(string key);

        public string? GetContent(string key, string value);
    }
}
