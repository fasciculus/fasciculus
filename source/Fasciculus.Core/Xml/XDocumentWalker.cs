using System.Xml.Linq;

namespace Fasciculus.Xml
{
    /// <summary>
    /// Walking through the elements of a <see cref="XDocument"/>
    /// </summary>
    public class XDocumentWalker : XElementWalker
    {
        /// <summary>
        /// Visits the root of the document (if any).
        /// </summary>
        public virtual void Visit(XDocument document)
        {
            XElement? root = document.Root;

            if (root != null)
            {
                Visit(root);
            }
        }
    }
}
