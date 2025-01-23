using System.Xml.Linq;

namespace Fasciculus.CodeAnalysis.Commenting
{
    public interface ICommentResolver
    {
        public XElement? Resolve(XElement? element);
    }
}
