using Fasciculus.CodeAnalysis.Models;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Fasciculus.CodeAnalysis.Compilers
{
    public class CompilationUnitCompiler : CSharpSyntaxWalker
    {
        public static CompilationUnit Compile(CompilationUnitSyntax compilationUnit)
        {
            return new();
        }
    }
}
