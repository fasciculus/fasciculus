namespace Fasciculus.Site.Rendering.Models
{
    public class FrontMatterEntry
    {
        public string Label { get; }

        public string Value { get; }

        public FrontMatterEntry(string label, string value)
        {
            Label = label;
            Value = value;
        }
    }
}
