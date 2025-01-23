using System.Xml.Linq;

namespace Fasciculus.Xml
{
    /// <summary>
    /// Extensions 
    /// </summary>
    public static class XObjectExtensions
    {
        /// <summary>
        /// Accepts the given <paramref name="visitor"/> by calling it's <see cref="IXVisitor.VisitObject(XObject)"/>.
        /// </summary>
        /// <param name="object"></param>
        /// <param name="visitor"></param>
        public static void Accept(this XObject @object, IXVisitor visitor)
            => visitor.VisitObject(@object);
    }
}
