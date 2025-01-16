using System.Text;

namespace Fasciculus.CodeAnalysis.Models
{
    public class SymbolModifiers
    {
        public bool IsPublic { get; set; }

        public bool IsPrivate { get; set; }

        public bool IsProtected { get; set; }

        public bool IsInternal { get; set; }

        public bool IsAccessible => IsPublic || IsProtected;

        public bool IsAbstract { get; set; }

        public bool IsStatic { get; set; }

        public bool IsReadonly { get; set; }

        public bool IsVirtual { get; set; }

        public bool IsOverride { get; set; }

        public bool IsUnsafe { get; set; }

        public bool IsAsync { get; set; }

        public bool IsPartial { get; set; }

        public override string? ToString()
        {
            StringBuilder sb = new();

            if (IsPublic) sb.Append("public ");
            if (IsPrivate) sb.Append("private ");
            if (IsProtected) sb.Append("protected ");
            if (IsInternal) sb.Append("internal ");
            if (IsAbstract) sb.Append("abstract ");
            if (IsStatic) sb.Append("static ");
            if (IsReadonly) sb.Append("readonly ");
            if (IsVirtual) sb.Append("virtual ");
            if (IsOverride) sb.Append("override ");
            if (IsUnsafe) sb.Append("unsafe ");
            if (IsAsync) sb.Append("async ");
            if (IsPartial) sb.Append("partial ");

            return sb.ToString().TrimEnd();
        }
    }
}
