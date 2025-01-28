using System.Text;

namespace Fasciculus.CodeAnalysis.Models
{
    public interface ISymbolModifiers
    {
        public bool IsPublic { get; }

        public bool IsPrivate { get; }

        public bool IsProtected { get; }

        public bool IsInternal { get; }

        public bool IsAccessible { get; }

        public bool IsAbstract { get; }

        public bool IsStatic { get; }

        public bool IsReadonly { get; }

        public bool IsVirtual { get; }

        public bool IsOverride { get; }

        public bool IsUnsafe { get; }

        public bool IsAsync { get; }

        public bool IsPartial { get; }
    }

    internal class SymbolModifiers : ISymbolModifiers
    {
        public bool IsPublic { get; set; }

        public bool IsPrivate { get; set; }

        public bool IsProtected { get; set; }

        public bool IsInternal { get; set; }

        public bool IsAccessible
            => (IsPublic || IsProtected) || !(IsPrivate || IsInternal);

        public bool IsAbstract { get; set; }

        public bool IsStatic { get; set; }

        public bool IsReadonly { get; set; }

        public bool IsVirtual { get; set; }

        public bool IsOverride { get; set; }

        public bool IsUnsafe { get; set; }

        public bool IsAsync { get; set; }

        public bool IsPartial { get; set; }

        public static SymbolModifiers Public()
            => new() { IsPublic = true };

        public SymbolModifiers() { }

        public SymbolModifiers(SymbolModifiers other)
        {
            IsPublic = other.IsPublic;
            IsPrivate = other.IsPrivate;
            IsProtected = other.IsProtected;
            IsInternal = other.IsInternal;
            IsStatic = other.IsStatic;
            IsReadonly = other.IsReadonly;
            IsVirtual = other.IsVirtual;
            IsOverride = other.IsOverride;
            IsUnsafe = other.IsUnsafe;
            IsAsync = other.IsAsync;
            IsPartial = other.IsPartial;
        }

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
