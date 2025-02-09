using System;
using System.Collections.Generic;

namespace Fasciculus.Markdown.FrontMatter
{
    public class FrontMapperMappings : IFrontMapperMappings
    {
        private readonly Dictionary<string, string> labels = [];
        private readonly Dictionary<string, Func<string, string>> labelConverters = [];

        private readonly Dictionary<string, Func<string, string>> contentConverters = [];

        public bool UseDefaultContent { get; set; } = true;

        public string? GetLabel(string key)
        {
            if (labels.TryGetValue(key, out string? label))
            {
                return label;
            }

            if (labelConverters.TryGetValue(key, out Func<string, string>? converter))
            {
                return converter(key);
            }

            return null;
        }

        public string? GetContent(string key, string value)
        {
            if (labelConverters.TryGetValue(key, out Func<string, string>? converter))
            {
                return converter(value);
            }

            return UseDefaultContent ? value : null;
        }

        public FrontMapperMappings WithLabel(string key, string label)
        {
            labels.Remove(key);
            labelConverters.Remove(key);

            labels.Add(key, label);

            return this;
        }

        public FrontMapperMappings WithLabel(string key, Func<string, string> converter)
        {
            labels.Remove(key);
            labelConverters.Remove(key);

            labelConverters.Add(key, converter);

            return this;
        }

        public FrontMapperMappings WithContent(string key, Func<string, string> converter)
        {
            contentConverters.Remove(key);
            contentConverters.Add(key, converter);

            return this;
        }

        public static FrontMapperMappings Empty()
            => new();
    }
}
