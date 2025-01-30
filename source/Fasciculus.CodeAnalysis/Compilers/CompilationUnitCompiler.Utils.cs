using Fasciculus.CodeAnalysis.Models;
using Fasciculus.Support;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Linq;

namespace Fasciculus.CodeAnalysis.Compilers
{
    internal partial class CompilationUnitCompiler
    {
        private bool IsIncluded(SymbolModifiers modifiers)
            => includeNonAccessible || modifiers.IsAccessible;

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
            string untyped = identifier.Text;

            if (typeParameters is null || typeParameters.Parameters.Count == 0)
            {
                return new(untyped);
            }

            string[] parameters = [.. typeParameters.Parameters.Select(p => p.Identifier.Text)];
            string name = $"{untyped}<{string.Join(',', parameters)}>";
            string mangled = $"{untyped}-{parameters.Count()}";

            return new(name, mangled);
        }

        private static SymbolName GetTypeName(TypeSyntax? type)
        {
            if (type is not null)
            {
                if (type is PredefinedTypeSyntax predefined) return GetPredefinedTypeName(predefined);
                if (type is IdentifierNameSyntax identifier) return GetIdentifierTypeName(identifier);
                if (type is GenericNameSyntax generic) return GetGenericTypeName(generic);
                if (type is NullableTypeSyntax nullable) return GetNullableTypeName(nullable);
                if (type is ArrayTypeSyntax array) return GetArrayTypeName(array);

                throw Ex.InvalidOperation("unhandled type");
            }

            throw Ex.InvalidOperation("type is null");
        }

        private static SymbolName GetPredefinedTypeName(PredefinedTypeSyntax predefined)
            => new(predefined.Keyword.ToString());

        private static SymbolName GetIdentifierTypeName(IdentifierNameSyntax identifier)
            => new(identifier.Identifier.ValueText);

        private static SymbolName GetGenericTypeName(GenericNameSyntax generic)
        {
            string untyped = generic.Identifier.ValueText;
            TypeSyntax[] args = [.. generic.TypeArgumentList.Arguments];
            SymbolName[] names = [.. args.Select(GetTypeName)];

            string name = $"{untyped}<{string.Join(',', names.Select(n => n.Name))}>";
            string mangled = $"{untyped}-{names.Length}";

            return new(name, mangled);
        }

        private static SymbolName GetNullableTypeName(NullableTypeSyntax nullable)
        {
            SymbolName elementName = GetTypeName(nullable.ElementType);

            string name = $"{elementName.Name}?";
            string mangled = $"{elementName.Mangled}-opt";

            return new(name, mangled);
        }

        private static SymbolName GetArrayTypeName(ArrayTypeSyntax array)
        {
            SymbolName elementName = GetTypeName(array.ElementType);

            string name = $"{elementName.Name}[]";
            string mangled = $"{elementName.Mangled}-array";

            return new(name, mangled);
        }
    }
}