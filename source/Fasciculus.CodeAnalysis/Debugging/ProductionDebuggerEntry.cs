using Fasciculus.Collections;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Fasciculus.CodeAnalysis.Debugging
{
    public class ProductionDebuggerEntry : IEquatable<ProductionDebuggerEntry>, IComparable<ProductionDebuggerEntry>
    {
        public static readonly IComparer<SyntaxKind> Comparer = Comparer<SyntaxKind>.Default;

        public SyntaxKind Left { get; }

        private readonly List<SyntaxKind> right;

        public IEnumerable<SyntaxKind> Right => right;

        public bool HasStructuredTrivia { get; }

        public ProductionDebuggerEntry(SyntaxNode node)
        {
            Left = node.Kind();
            right = new(node.ChildNodes().Select(n => n.Kind()));
            HasStructuredTrivia = node.HasStructuredTrivia;
        }

        public bool Equals(ProductionDebuggerEntry? other)
        {
            if (other is null)
            {
                return false;
            }

            if (Left != other.Left)
            {
                return false;
            }

            return right.SequenceEqual(other.right);
        }

        public override bool Equals(object? obj)
            => Equals(obj as ProductionDebuggerEntry);

        public override int GetHashCode()
            => Left.GetHashCode() ^ right.ToHashCode();

        public int CompareTo(ProductionDebuggerEntry? other)
        {
            if (other is null)
            {
                return -1;
            }

            int result = Left.CompareTo(other.Left);

            if (result == 0)
            {
                result = right.SequenceCompare(other.right, Comparer);
            }

            return result;
        }

        public override string? ToString()
        {
            string l = Left.ToString();
            string r = string.Join(" ", right.Select(n => n.ToString()));

            return $"{l}: {r}";
        }
    }
}
