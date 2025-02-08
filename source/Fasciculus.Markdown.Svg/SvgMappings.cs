using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace Fasciculus.Markdown.Svg
{
    public class SvgMappings : ISvgMappings
    {
        private Dictionary<string, XElement> elements = [];
        private Dictionary<string, Func<XElement>> factories = [];

        public void Add(string key, XElement element)
        {
            elements.Remove(key);
            factories.Remove(key);

            elements.Add(key, element);
        }

        public void Add(string key, Func<XElement> factory)
        {
            elements.Remove(key);
            factories.Remove(key);

            factories.Add(key, factory);
        }

        public XElement? GetSvg(string key)
        {
            if (elements.TryGetValue(key, out XElement? element))
            {
                return element;
            }

            if (factories.TryGetValue(key, out Func<XElement>? factory))
            {
                return factory();
            }

            return null;
        }
    }
}
