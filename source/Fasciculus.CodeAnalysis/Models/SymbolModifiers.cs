namespace Fasciculus.CodeAnalysis.Models
{
    public class SymbolModifiers
    {
        public bool IsPublic { get; set; }

        public bool IsAccessible => IsPublic;

        public bool IsAbstract { get; set; }

        public bool IsStatic { get; set; }

        public bool IsPartial { get; set; }
    }
}
