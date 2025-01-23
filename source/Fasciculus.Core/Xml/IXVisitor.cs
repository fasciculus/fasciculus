using System.Collections.Generic;
using System.Xml.Linq;

namespace Fasciculus.Xml
{
    /// <summary>
    /// Visitor to visit XDocument nodes.
    /// </summary>
    public interface IXVisitor
    {
        /// <summary>
        /// Visit the given <paramref name="document"/>.
        /// </summary>
        public void VisitDocument(XDocument document);

        /// <summary>
        /// Visit the given <paramref name="element"/>.
        /// </summary>
        public void VisitElement(XElement element);

        /// <summary>
        /// Visit the given <paramref name="nodes"/>.
        /// </summary>
        public void VisitNodes(IEnumerable<XNode> nodes);

        /// <summary>
        /// Visit the given <paramref name="node"/>.
        /// </summary>
        public void VisitNode(XNode node);

        /// <summary>
        /// Visit the given <paramref name="cData"/>.
        /// </summary>
        public void VisitCData(XCData cData);

        /// <summary>
        /// Visit the given <paramref name="text"/>.
        /// </summary>
        public void VisitText(XText text);

        /// <summary>
        /// Visit the given <paramref name="comment"/>.
        /// </summary>
        public void VisitComment(XComment comment);

        /// <summary>
        /// Visit the given <paramref name="documentType"/>.
        /// </summary>
        public void VisitDocumentType(XDocumentType documentType);

        /// <summary>
        /// Visit the given <paramref name="processingInstruction"/>.
        /// </summary>
        public void VisitProcessingInstruction(XProcessingInstruction processingInstruction);

        /// <summary>
        /// Visit the given <paramref name="attributes"/>.
        /// </summary>
        public void VisitAttributes(IEnumerable<XAttribute> attributes);

        /// <summary>
        /// Visit the given <paramref name="attribute"/>.
        /// </summary>
        public void VisitAttribute(XAttribute attribute);

        /// <summary>
        /// Visits the given <paramref name="object"/>.
        /// </summary>
        public void VisitObject(XObject @object);
    }
}
