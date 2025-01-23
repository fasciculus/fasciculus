using System.Xml.Linq;

namespace Fasciculus.CodeAnalysis.Commenting
{
    public interface ICommentFormatter
    {
        public string Format(XElement? element);
    }
}
