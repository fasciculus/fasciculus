using System;

namespace Fasciculus.Svg.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class SvgAttribute : Attribute
    {
        public string Key { get; }

        public SvgAttribute(string key)
        {
            Key = key;
        }
    }
}
