using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Xml;
using System.Xml.Linq;

namespace Fasciculus.Xml
{
    /// <summary>
    /// Default impementation of <see cref="IXVisitor"/>.
    /// </summary>
    public class XVisitor : IXVisitor
    {
        /// <summary>
        /// Visit the given <paramref name="document"/>.
        /// </summary>
        /// <remarks>
        /// Visits the document's nodes.
        /// </remarks>
        public virtual void VisitDocument(XDocument document)
        {
            VisitNodes(document.Nodes());
        }

        /// <summary>
        /// Visit the given <paramref name="element"/>.
        /// </summary>
        /// <remarks>
        /// Visits the element's attributes and nodes.
        /// </remarks>
        public virtual void VisitElement(XElement element)
        {
            VisitAttributes(element.Attributes());
            VisitNodes(element.Nodes());
        }

        /// <summary>
        /// Visits the given <paramref name="nodes"/>.
        /// </summary>
        /// <remarks>
        /// The nodes are stored in an array in case of the tree being manipulated.
        /// </remarks>
        public virtual void VisitNodes(IEnumerable<XNode> nodes)
        {
            XNode[] asArray = [.. nodes];

            foreach (XNode node in asArray)
            {
                node.Accept(this);
            }
        }

        /// <summary>
        /// Visit the given <paramref name="node"/>.
        /// </summary>
        public virtual void VisitNode(XNode node)
        {
            switch (node.NodeType)
            {
                case XmlNodeType.CDATA: VisitNode<XCData>(node, VisitCData); break;
                case XmlNodeType.Comment: VisitNode<XComment>(node, VisitComment); break;
                case XmlNodeType.Document: VisitNode<XDocument>(node, VisitDocument); break;
                case XmlNodeType.DocumentType: VisitNode<XDocumentType>(node, VisitDocumentType); break;
                case XmlNodeType.Element: VisitNode<XElement>(node, VisitElement); break;
                case XmlNodeType.ProcessingInstruction: VisitNode<XProcessingInstruction>(node, VisitProcessingInstruction); break;
                case XmlNodeType.Text: VisitNode<XText>(node, VisitText); break;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void VisitNode<T>(XNode node, Action<T> action)
            where T : XNode
        {
            if (node is T typedNode)
            {
                action(typedNode);
            }
        }

        /// <summary>
        /// Visit the given <paramref name="cData"/>.
        /// </summary>
        /// <remarks>
        /// Does nothing unless overridden.
        /// </remarks>
        public virtual void VisitCData(XCData cData) { }

        /// <summary>
        /// Visit the given <paramref name="text"/>.
        /// </summary>
        /// <remarks>
        /// Does nothing unless overridden.
        /// </remarks>
        public virtual void VisitText(XText text) { }

        /// <summary>
        /// Visit the given <paramref name="comment"/>.
        /// </summary>
        /// <remarks>
        /// Does nothing unless overridden.
        /// </remarks>
        public virtual void VisitComment(XComment comment) { }

        /// <summary>
        /// Visit the given <paramref name="documentType"/>.
        /// </summary>
        /// <remarks>
        /// Does nothing unless overridden.
        /// </remarks>
        public virtual void VisitDocumentType(XDocumentType documentType) { }

        /// <summary>
        /// Visit the given <paramref name="processingInstruction"/>.
        /// </summary>
        /// <remarks>
        /// Does nothing unless overridden.
        /// </remarks>
        public virtual void VisitProcessingInstruction(XProcessingInstruction processingInstruction) { }

        /// <summary>
        /// Visits the given <paramref name="attributes"/>.
        /// </summary>
        /// <remarks>
        /// The attributes are stored in an array in case of the tree being manipulated.
        /// </remarks>
        public virtual void VisitAttributes(IEnumerable<XAttribute> attributes)
        {
            XAttribute[] asArray = [.. attributes];

            foreach (XAttribute attribute in asArray)
            {
                attribute.Accept(this);
            }
        }

        /// <summary>
        /// Visit the given <paramref name="attribute"/>.
        /// </summary>
        /// <remarks>
        /// Does nothing unless overridden.
        /// </remarks>
        public virtual void VisitAttribute(XAttribute attribute) { }

        /// <summary>
        /// Visits the given <paramref name="object"/>.
        /// </summary>
        public virtual void VisitObject(XObject @object)
        {
            if (@object.NodeType == XmlNodeType.Attribute)
            {
                if (@object is XAttribute attribute)
                {
                    VisitAttribute(attribute);
                }
            }
            else
            {
                if (@object is XNode node)
                {
                    VisitNode(node);
                }
            }
        }
    }
}
