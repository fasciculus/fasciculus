using Fasciculus.Markdown.Extensions.Svg;
using Fasciculus.Svg.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;

namespace Fasciculus.Blog.Services
{
    public class Graphics : ISvgMappings
    {
        private readonly Dictionary<string, MethodInfo> methods = [];

        public Graphics()
        {
            Type[] types = typeof(Graphics).Assembly.GetTypes();
            MethodInfo[] methods = [.. types.SelectMany(type => type.GetMethods())];

            foreach (MethodInfo method in methods)
            {
                MethodAttributes attributes = method.Attributes;

                if (!attributes.HasFlag(MethodAttributes.Static)) continue;

                SvgAttribute? attribute = method.GetCustomAttribute<SvgAttribute>();

                if (attribute is null) continue;

                string key = attribute.Key;

                this.methods[key] = method;
            }
        }

        public XElement? GetSvg(string key)
        {
            if (methods.TryGetValue(key, out MethodInfo? method))
            {
                return method.Invoke(null, null) as XElement;
            }

            return null;
        }
    }
}
