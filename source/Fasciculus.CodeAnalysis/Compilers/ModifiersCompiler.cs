using Fasciculus.CodeAnalysis.Debugging;
using Fasciculus.CodeAnalysis.Models;
using Fasciculus.Collections;
using Fasciculus.Threading.Synchronization;
using Microsoft.CodeAnalysis;

namespace Fasciculus.CodeAnalysis.Compilers
{
    public class ModifiersCompiler
    {
        private readonly IModifierDebugger debugger;

        private readonly TaskSafeMutex mutex = new();

        private SymbolModifiers modifiers = new();

        public ModifiersCompiler(CompilerContext context)
        {
            debugger = context.Debuggers.ModifierDebugger;
        }

        public SymbolModifiers Compile(SyntaxTokenList tokens)
        {
            using Locker locker = Locker.Lock(mutex);
            modifiers = new();

            tokens.Apply(VisitToken);

            return modifiers;
        }

        private void VisitToken(SyntaxToken token)
        {
            string name = token.Text;

            debugger.Add(name);

            switch (name)
            {
                case "public": modifiers.IsPublic = true; break;
                case "private": modifiers.IsPrivate = true; break;
                case "protected": modifiers.IsProtected = true; break;
                case "readonly": modifiers.IsReadonly = true; break;
                case "abstract": modifiers.IsAbstract = true; break;
                case "static": modifiers.IsStatic = true; break;
                case "partial": modifiers.IsPartial = true; break;
            }
        }
    }
}
