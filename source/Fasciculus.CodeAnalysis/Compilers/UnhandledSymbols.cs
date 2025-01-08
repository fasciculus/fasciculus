using Fasciculus.Collections;
using Fasciculus.Threading.Synchronization;
using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Fasciculus.CodeAnalysis.Compilers
{
    public class UnhandledSymbols
    {
        public static readonly UnhandledSymbols Instance = new();

        private readonly TaskSafeMutex mutex = new();

        private readonly Dictionary<string, SortedSet<SyntaxKind>> handled = [];
        private readonly Dictionary<string, SortedSet<SyntaxKind>> used = [];

        private UnhandledSymbols() { }

        public void Handled(string type, IEnumerable<SyntaxKind> symbols)
        {
            using Locker locker = Locker.Lock(mutex);

            if (!handled.ContainsKey(type))
            {
                handled.Add(type, new(symbols));
                used.Add(type, []);
            }
        }

        public void Handled(Type type, IEnumerable<SyntaxKind> symbols)
            => Handled(type.FullName ?? type.Name, symbols);

        public void Used(string type, SyntaxKind symbol)
        {
            using Locker locker = Locker.Lock(mutex);

            if (used.TryGetValue(type, out SortedSet<SyntaxKind>? symbols))
            {
                symbols.Add(symbol);
            }
        }

        public void Used(Type type, SyntaxKind kind)
            => Used(type.FullName ?? type.Name, kind);

        public Dictionary<string, SortedSet<SyntaxKind>> Unhandled()
        {
            using Locker locker = Locker.Lock(mutex);

            return used
                .Select(kvp => Tuple.Create(kvp.Key, Unhandled(kvp.Key)))
                .Where(t => t.Item2.Count > 0)
                .ToDictionary();
        }

        private SortedSet<SyntaxKind> Unhandled(string type)
        {
            SortedSet<SyntaxKind> unhandled = new(used[type]);

            unhandled.ExceptWith(handled[type]);

            return unhandled;
        }
    }
}
