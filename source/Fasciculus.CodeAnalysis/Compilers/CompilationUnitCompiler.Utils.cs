using Fasciculus.CodeAnalysis.Models;
using Fasciculus.Collections;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Linq;

namespace Fasciculus.CodeAnalysis.Compilers
{
    internal partial class CompilationUnitCompiler
    {
        private SymbolModifiers GetModifiers(SyntaxTokenList tokens)
        {
            SymbolModifiers modifiers = new();

            foreach (SyntaxToken token in tokens)
            {
                string name = token.Text;

                modifierDebugger.Add(name);

                switch (name)
                {
                    case "public": modifiers.IsPublic = true; break;
                    case "private": modifiers.IsPrivate = true; break;
                    case "protected": modifiers.IsProtected = true; break;
                    case "internal": modifiers.IsInternal = true; break;
                    case "abstract": modifiers.IsAbstract = true; break;
                    case "static": modifiers.IsStatic = true; break;
                    case "readonly": modifiers.IsReadonly = true; break;
                    case "virtual": modifiers.IsVirtual = true; break;
                    case "override": modifiers.IsOverride = true; break;
                    case "unsafe": modifiers.IsUnsafe = true; break;
                    case "async": modifiers.IsAsync = true; break;
                    case "partial": modifiers.IsPartial = true; break;
                }
            }

            return modifiers;
        }

        private static SymbolName GetName(SyntaxToken identifier, TypeParameterListSyntax? typeParameters)
        {
            string name = identifier.Text;

            if (typeParameters is null || typeParameters.Parameters.Count == 0)
            {
                return new(name);
            }

            string[] parameters = [.. typeParameters.Parameters.Select(p => p.Identifier.Text)];

            return new(name, parameters);
        }

        private static SymbolName GetName(SyntaxToken identifier, ParameterListSyntax? parameters, TypeParameterListSyntax? typeParameters)
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

        private static string GetTypeName(TypeSyntax type)
        {
            if (type is PredefinedTypeSyntax predefined) return GetPredefinedTypeName(predefined);
            if (type is IdentifierNameSyntax identifier) return GetIdentifierTypeName(identifier);
            if (type is GenericNameSyntax generic) return GetGenericTypeName(generic);
            if (type is NullableTypeSyntax nullable) return GetNullableTypeName(nullable);

            return string.Empty;
        }

        private static string GetPredefinedTypeName(PredefinedTypeSyntax type)
        {
            return type.Keyword.ToString();
        }

        private static string GetIdentifierTypeName(IdentifierNameSyntax identifier)
        {
            return identifier.Identifier.ValueText;
        }

        private static string GetGenericTypeName(GenericNameSyntax type)
        {
            string name = type.Identifier.ValueText;
            TypeSyntax[] args = [.. type.TypeArgumentList.Arguments];
            string[] names = [.. args.Select(GetTypeName)];

            return $"{name}<{string.Join(",", names)}>";
        }

        private static string GetNullableTypeName(NullableTypeSyntax nullable)
        {
            return GetTypeName(nullable.ElementType) + "?";
        }
    }
}