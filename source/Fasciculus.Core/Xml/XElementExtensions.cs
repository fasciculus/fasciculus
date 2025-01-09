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
        /// Returns the inner XML of the given <paramref name="element"/>.
        /// </summary>
        public static string InnerXml(this XElement? element)
        {
            if (element is not null)
            {
                using XmlReader reader = element.CreateReader();

                reader.MoveToContent();

                return reader.ReadInnerXml();
            }

            return string.Empty;
        }
    }
}
