using System;
using System.Xml;
using System.Xml.Linq;

namespace Fasciculus.Xml
{
    /// <summary>
    /// Extensions for <see cref="XElement"/>.
    /// </summary>
    public static class XElementExtensions
    {
        /// <summary>
        /// Opens a reader for the given <paramref name="element"/>, passes it to the given <paramref name="read"/> function,
        /// closes the reader and returns the <paramref name="read"/>'s result.
        /// </summary>
        public static string ReadContent(this XElement? element, Func<XmlReader, string> read)
        {
            if (element is not null)
            {
                using XmlReader reader = element.CreateReader();

                reader.MoveToContent();

                return read(reader);
            }

            return string.Empty;
        }

        /// <summary>
        /// Returns the inner XML of the given <paramref name="element"/>.
        /// </summary>
        public static string InnerXml(this XElement? element)
            => element.ReadContent(r => r.ReadInnerXml());
    }
}
