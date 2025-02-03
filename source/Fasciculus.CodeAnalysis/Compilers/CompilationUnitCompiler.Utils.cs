using Fasciculus.CodeAnalysis.Models;
using Fasciculus.Collections;
using Fasciculus.Net.Navigating;
using Fasciculus.Support;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Linq;

namespace Fasciculus.CodeAnalysis.Compilers
{
    internal partial class CompilationUnitCompiler
    {
        private bool IsIncluded(SymbolModifiers modifiers)
            => IncludeNonAccessible || modifiers.IsAccessible;

        private SymbolModifiers GetModifiers(SyntaxTokenList tokens)
        {
            string[] names = [.. tokens.Select(t => t.Text)];

            names.Apply(ModifierDebugger.Add);

            return SymbolModifiers.Parse(names);
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
            string mangled = $"{untyped}-{parameters.Length}";

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

        private static UriPath GetParameterLink(ParameterSyntax node)
        {
            TypeSyntax type = Cond.NotNull(node.Type);
            string name = node.Identifier.ValueText;
            IEnumerable<string> suffixes = GetParameterSuffixes(type);
            string path = string.Join("-", suffixes.Prepend(name));

            return new(path);
        }

        private static IEnumerable<string> GetParameterSuffixes(TypeSyntax type)
        {
            return type switch
            {
                PredefinedTypeSyntax predefined => GetPredefinedParameterSuffixes(predefined),
                IdentifierNameSyntax identifier => GetIdentifierParameterSuffixes(identifier),
                GenericNameSyntax generic => GetGenericParameterSuffixes(generic),
                NullableTypeSyntax nullable => GetNullableParameterSuffixes(nullable),
                ArrayTypeSyntax array => GetArrayParameterSuffixes(array),
                _ => throw Ex.InvalidOperation($"{type.GetType()}")
            };
        }

        private static IEnumerable<string> GetGenericParameterSuffixes(GenericNameSyntax generic)
        {
            string untyped = generic.Identifier.ValueText;
            IEnumerable<TypeSyntax> args = generic.TypeArgumentList.Arguments;
            IEnumerable<string> suffixes = args.SelectMany(GetParameterSuffixes);

            return suffixes.Prepend(untyped);
        }

        private static IEnumerable<string> GetPredefinedParameterSuffixes(PredefinedTypeSyntax predefined)
            => [predefined.Keyword.ToString()];

        private static IEnumerable<string> GetIdentifierParameterSuffixes(IdentifierNameSyntax identifier)
            => [identifier.Identifier.ValueText];

        private static IEnumerable<string> GetNullableParameterSuffixes(NullableTypeSyntax nullable)
            => GetParameterSuffixes(nullable.ElementType).Append(".opt");

        private static IEnumerable<string> GetArrayParameterSuffixes(ArrayTypeSyntax array)
            => GetParameterSuffixes(array.ElementType).Append(".array");
    }
}