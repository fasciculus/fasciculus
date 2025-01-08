using System.Diagnostics;
using System.Xml.Linq;

namespace Fasciculus.CodeAnalysis.Commenting
{
    public class SymbolCommentFactory
    {
        public SymbolComment Create(string xml)
        {
            try
            {
                XDocument document = XDocument.Parse(xml);

                return new(document);
            }
            catch
            {
                Debugger.Break();

                return SymbolComment.Empty;
            }
        }

        public static SymbolComment CreateEmpty()
        {
            SymbolCommentFactory factory = new();

            return factory.Create("<comment />");
        }
    }
}
