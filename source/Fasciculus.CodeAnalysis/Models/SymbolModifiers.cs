using System.Text;

namespace Fasciculus.CodeAnalysis.Models
{
    public class SymbolModifiers
    {
        public bool IsPublic { get; set; }

        public bool IsAccessible => IsPublic;

        public bool IsAbstract { get; set; }

        public bool IsStatic { get; set; }

        public bool IsPartial { get; set; }

        public override string? ToString()
        {
            StringBuilder sb = new();

            if (IsPublic) sb.Append("public ");
            if (IsAbstract) sb.Append("abstract ");
            if (IsStatic) sb.Append("static ");
            if (IsPartial) sb.Append("partial ");

            return sb.ToString().TrimEnd();
        }
    }
}
