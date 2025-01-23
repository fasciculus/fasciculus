using System.Xml.Linq;
using static Fasciculus.CodeAnalysis.Commenting.CommentConstants;

namespace Fasciculus.CodeAnalysis.Commenting
{
    public class DefaultCommentMerger : ICommentMerger
    {
        public void Merge(XDocument src, XDocument dst)
        {
            XElement dstRoot = EnsureRoot(dst);
            XElement? srcRoot = src.Root;

            Merge(srcRoot, dstRoot, SummaryName);
            Merge(srcRoot, dstRoot, RemarksName);
        }

        private static void Merge(XElement? srcRoot, XElement dstRoot, string name)
        {
            XElement? dst = dstRoot.Element(name);

            if (dst is null)
            {
                XElement? src = srcRoot?.Element(name);

                if (src is not null)
                {
                    dstRoot.Add(new XElement(src));
                }
            }
        }


        private static XElement EnsureRoot(XDocument doc)
        {
            XElement? root = doc.Root;

            if (root is null)
            {
                root = new(RootName);

                doc.Add(root);
            }

            return root;
        }
    }
}
