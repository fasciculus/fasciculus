using System.Xml.Linq;

namespace Fasciculus.CodeAnalysis.Commenting
{
    public interface ICommentMerger
    {
        public void Merge(XDocument src, XDocument dst);
    }
}
