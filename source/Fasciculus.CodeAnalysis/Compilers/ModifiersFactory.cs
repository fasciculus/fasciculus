using Fasciculus.CodeAnalysis.Models;
using Fasciculus.Collections;
using Fasciculus.Threading.Synchronization;
using Microsoft.CodeAnalysis;

namespace Fasciculus.CodeAnalysis.Compilers
{
    public class ModifiersFactory
    {
        private static readonly string[] HandledModifiers
            = ["public", "abstract", "static", "partial"];

        private readonly TaskSafeMutex mutex = new();

        SymbolModifiers modifiers = new();

        public ModifiersFactory()
        {
            UnhandledModifiers.Instance.Handled(HandledModifiers);
        }

        public SymbolModifiers Create(SyntaxTokenList tokens)
        {
            using Locker locker = Locker.Lock(mutex);

            modifiers = new();

            tokens.Apply(VisitToken);

            return modifiers;
        }

        private void VisitToken(SyntaxToken token)
        {
            string name = token.Text;

            UnhandledModifiers.Instance.Used(name);

            switch (name)
            {
                case "public": modifiers.IsPublic = true; break;
                case "abstract": modifiers.IsAbstract = true; break;
                case "static": modifiers.IsStatic = true; break;
                case "partial": modifiers.IsPartial = true; break;
            }
        }
    }
}
