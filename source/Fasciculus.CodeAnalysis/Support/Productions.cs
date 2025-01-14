using Fasciculus.CodeAnalysis.Debugging;
using Fasciculus.Collections;
using Fasciculus.Threading.Synchronization;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Fasciculus.CodeAnalysis.Support
{
    public class Production : IEquatable<Production>, IComparable<Production>
    {
        public static readonly IComparer<SyntaxKind> Comparer = Comparer<SyntaxKind>.Default;

        public SyntaxKind Left { get; }

        private readonly List<SyntaxKind> right;

        public IEnumerable<SyntaxKind> Right => right;

        public bool HasTrivia { get; }

        public Production(SyntaxNode node)
        {
            Left = node.Kind();
            right = new(node.ChildNodes().Select(n => n.Kind()));
            HasTrivia = node.HasStructuredTrivia;
        }

        public bool Equals(Production? other)
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
            => Equals(obj as Production);

        public override int GetHashCode()
            => Left.GetHashCode() ^ right.ToHashCode();

        public int CompareTo(Production? other)
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

    public class Productions : INodeLogger
    {
        public static readonly Productions Instance = new();

        private readonly TaskSafeMutex mutex = new();
        private readonly SortedSet<Production> productions = [];

        private Productions() { }

        public void Add(SyntaxNode node)
        {
            using Locker locker = Locker.Lock(mutex);

            productions.Add(new(node));
            Syntax.Instance.Add(node);
        }

        public List<Production> GetProductions(SyntaxKind left)
        {
            using Locker locker = Locker.Lock(mutex);

            return new(productions.Where(p => p.Left == left));
        }
    }
}
