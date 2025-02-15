using System.Collections.Generic;
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

        public bool IsSealed { get; }

        public bool IsReadonly { get; }

        public bool IsVirtual { get; }

        public bool IsOverride { get; }

        public bool IsUnsafe { get; }

        public bool IsAsync { get; }

        public bool IsPartial { get; }

        public bool IsParams { get; }

        public bool IsThis { get; }

        public bool IsOut { get; set; }
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

        public bool IsSealed { get; set; }

        public bool IsReadonly { get; set; }

        public bool IsVirtual { get; set; }

        public bool IsOverride { get; set; }

        public bool IsUnsafe { get; set; }

        public bool IsAsync { get; set; }

        public bool IsPartial { get; set; }

        public bool IsParams { get; set; }

        public bool IsThis { get; set; }

        public bool IsOut { get; set; }

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
            IsSealed = other.IsSealed;
            IsReadonly = other.IsReadonly;
            IsVirtual = other.IsVirtual;
            IsOverride = other.IsOverride;
            IsUnsafe = other.IsUnsafe;
            IsAsync = other.IsAsync;
            IsPartial = other.IsPartial;
            IsParams = other.IsParams;
            IsThis = other.IsThis;
            IsOut = other.IsOut;
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
            if (IsSealed) sb.Append("sealed ");
            if (IsReadonly) sb.Append("readonly ");
            if (IsVirtual) sb.Append("virtual ");
            if (IsOverride) sb.Append("override ");
            if (IsUnsafe) sb.Append("unsafe ");
            if (IsAsync) sb.Append("async ");
            if (IsPartial) sb.Append("partial ");
            if (IsPartial) sb.Append("params ");
            if (IsThis) sb.Append("this ");
            if (IsOut) sb.Append("out ");

            return sb.ToString().TrimEnd();
        }

        public static SymbolModifiers Parse(IEnumerable<string> names)
        {
            SymbolModifiers modifiers = new();

            foreach (string name in names)
            {
                switch (name)
                {
                    case "public": modifiers.IsPublic = true; break;
                    case "private": modifiers.IsPrivate = true; break;
                    case "protected": modifiers.IsProtected = true; break;
                    case "internal": modifiers.IsInternal = true; break;
                    case "abstract": modifiers.IsAbstract = true; break;
                    case "static": modifiers.IsStatic = true; break;
                    case "sealed": modifiers.IsSealed = true; break;
                    case "readonly": modifiers.IsReadonly = true; break;
                    case "virtual": modifiers.IsVirtual = true; break;
                    case "override": modifiers.IsOverride = true; break;
                    case "unsafe": modifiers.IsUnsafe = true; break;
                    case "async": modifiers.IsAsync = true; break;
                    case "partial": modifiers.IsPartial = true; break;
                    case "params": modifiers.IsParams = true; break;
                    case "this": modifiers.IsThis = true; break;
                    case "out": modifiers.IsOut = true; break;
                }
            }

            return modifiers;
        }
    }
}
