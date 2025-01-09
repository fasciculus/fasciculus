using Fasciculus.Collections;
using System.Xml.Linq;

namespace Fasciculus.Xml
{
    /// <summary>
    /// Walking through the elements of a <see cref="XDocument"/>
    /// </summary>
    public class XElementWalker
    {
        /// <summary>
        /// Visits the children of the given <paramref name="element"/>.
        /// </summary>
        public virtual void Visit(XElement element)
        {
            XElement[] children = [.. element.Elements()];

            children.Apply(Visit);
        }
    }
}
