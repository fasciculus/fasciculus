using Fasciculus.Collections;
using System.Collections.Generic;
using System.Xml.Linq;

namespace Fasciculus.Xml
{
    /// <summary>
    /// A walker to walk <see cref="XDocument"/>s.
    /// </summary>
    public class XWalker
    {
        /// <summary>
        /// Visits the nodes of the given <paramref name="document"/>
        /// </summary>
        public virtual void VisitDocument(XDocument document)
        {
            VisitNodes(document.Nodes());
        }

        /// <summary>
        /// Visits the attributes and then the nodes of the given <paramref name="element"/>
        /// </summary>
        public virtual void VisitElement(XElement element)
        {
            VisitAttributes(element.Attributes());
            VisitNodes(element.Nodes());
        }

        /// <summary>
        /// Visits the given <paramref name="attributes"/>.
        /// </summary>
        public virtual void VisitAttributes(IEnumerable<XAttribute> attributes)
        {
            XAttribute[] asArray = [.. attributes];

            asArray.Apply(VisitAttribute);
        }

        /// <summary>
        /// Does nothing unless overridden.
        /// </summary>
        public virtual void VisitAttribute(XAttribute attribute) { }

        /// <summary>
        /// Dispatches the given <paramref name="node"/> to the other Visit methods.
        /// </summary>
        public virtual void VisitNode(XNode node)
        {
            if (node is XText text)
            {
                if (node is XCData cData)
                {
                    VisitCData(cData);
                }
                else
                {
                    VisitText(text);
                }
            }
            else if (node is XElement element)
            {
                VisitElement(element);
            }
            else if (node is XComment comment)
            {
                VisitComment(comment);
            }
            else if (node is XDocumentType documentType)
            {
                VisitDocumentType(documentType);
            }
            else if (node is XProcessingInstruction processingInstruction)
            {
                VisitProcessingInstruction(processingInstruction);
            }
            else if (node is XDocument document)
            {
                VisitDocument(document);
            }
        }

        /// <summary>
        /// Visits the given <paramref name="nodes"/> in the given order.
        /// </summary>
        public virtual void VisitNodes(IEnumerable<XNode> nodes)
        {
            XNode[] asArray = [.. nodes];

            asArray.Apply(VisitNode);
        }

        /// <summary>
        /// Does nothing unless overridden.
        /// </summary>
        public virtual void VisitComment(XComment comment) { }

        /// <summary>
        /// Does nothing unless overridden.
        /// </summary>
        public virtual void VisitDocumentType(XDocumentType documentType) { }

        /// <summary>
        /// Does nothing unless overridden.
        /// </summary>
        public virtual void VisitProcessingInstruction(XProcessingInstruction processingInstruction) { }

        /// <summary>
        /// Does nothing unless overridden.
        /// </summary>
        public virtual void VisitCData(XCData cData) { }

        /// <summary>
        /// Does nothing unless overridden.
        /// </summary>
        public virtual void VisitText(XText text) { }

    }
}
