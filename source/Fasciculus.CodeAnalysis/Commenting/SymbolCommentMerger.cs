using System.Xml.Linq;

namespace Fasciculus.CodeAnalysis.Commenting
{
    public static class SymbolCommentMerger
    {
        public static void Merge(this XDocument dst, XDocument src)
        {
            XElement dstRoot = EnsureRoot(dst);
            XElement? srcRoot = src.Root;

            Merge(dstRoot, srcRoot, "summary");
        }

        private static void Merge(XElement dstRoot, XElement? srcRoot, string name)
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

        private static XElement EnsureRoot(this XDocument doc)
        {
            XElement? root = doc.Root;

            if (root is null)
            {
                root = new("comment");

                doc.Add(root);
            }

            return root;
        }
    }
}
