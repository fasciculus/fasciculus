using System.Diagnostics;
using System.Xml;

namespace Fasciculus.CodeAnalysis.Commenting
{
    public class SymbolCommentFactory
    {
        public SymbolComment Create(string xml)
        {
            try
            {
                XmlDocument document = new();

                document.LoadXml(xml);

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
