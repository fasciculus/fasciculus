using Fasciculus.Markdown.Extensions.Svg;
using Fasciculus.Svg.Attributes;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Xml.Linq;

namespace Fasciculus.Docs.Content.Services
{
    public class GraphicsProvider : ISvgMappings
    {
        private readonly Dictionary<string, MethodInfo> methods = [];

        public GraphicsProvider()
        {
            foreach (Type type in typeof(GraphicsProvider).Assembly.GetTypes())
            {
                foreach (MethodInfo method in type.GetMethods())
                {
                    if (IsSvgMethod(method))
                    {
                        string key = method.GetCustomAttribute<SvgAttribute>()!.Key;

                        methods[key] = method;
                    }
                }
            }
        }

        private static bool IsSvgMethod(MethodInfo method)
        {
            bool result = true;

            result = result && method.IsStatic;
            result = result && method.IsPublic;
            result = result && method.ReturnType.IsAssignableTo(typeof(XElement));
            result = result && method.GetParameters().Length == 0;
            result = result && method.GetCustomAttribute<SvgAttribute>() != null;

            return result;
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
