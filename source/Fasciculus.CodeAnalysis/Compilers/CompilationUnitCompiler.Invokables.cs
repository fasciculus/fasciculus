using Fasciculus.CodeAnalysis.Models;
using Fasciculus.Collections;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Linq;

namespace Fasciculus.CodeAnalysis.Compilers
{
    public partial class CompilationUnitCompiler
    {
        private SymbolName GetName(SyntaxToken identifier, ParameterListSyntax? parameters, TypeParameterListSyntax? typeParameters)
        {
            SymbolName prefix = GetName(identifier, typeParameters);

            if (parameters is null || parameters.Parameters.Count == 0)
            {
                return prefix;
            }

            TypeSyntax[] types = [.. parameters.Parameters.Select(p => p.Type).NotNull()];
            string[] typeNames = [.. types.Select(t => t.ToFullString().TrimEnd())];

            string name = $"{prefix}({string.Join(",", typeNames)})";

            return new(name);
        }
    }
}