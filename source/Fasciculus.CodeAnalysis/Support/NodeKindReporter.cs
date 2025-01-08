using Fasciculus.Threading.Synchronization;
using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Collections.Generic;
using System.IO;

namespace Fasciculus.CodeAnalysis.Support
{
    public class NodeKindReporter
    {
        public static readonly NodeKindReporter Instance = new();

        private readonly TaskSafeMutex mutex = new();

        private readonly Dictionary<string, SortedSet<SyntaxKind>> accepted = [];
        private readonly Dictionary<string, SortedSet<SyntaxKind>> used = [];

        private NodeKindReporter() { }

        public void Add(string type, IEnumerable<SyntaxKind> acceptedKinds)
        {
            using Locker locker = Locker.Lock(mutex);

            if (!accepted.ContainsKey(type))
            {
                accepted.Add(type, new(acceptedKinds));
                used.Add(type, []);
            }
        }

        public void Add(Type type, IEnumerable<SyntaxKind> acceptedKinds)
            => Add(type.FullName ?? type.Name, acceptedKinds);

        public void Used(string type, SyntaxKind kind)
        {
            using Locker locker = Locker.Lock(mutex);

            if (used.TryGetValue(type, out SortedSet<SyntaxKind>? kinds))
            {
                kinds.Add(kind);
            }
        }

        public void Used(Type type, SyntaxKind kind)
            => Used(type.FullName ?? type.Name, kind);

        public bool Report(StringWriter writer)
        {
            using Locker locker = Locker.Lock(mutex);

            bool result = false;
            SortedSet<string> keys = new(accepted.Keys);

            foreach (string key in keys)
            {
                SortedSet<SyntaxKind> kinds = used[key];

                kinds.ExceptWith(accepted[key]);

                if (kinds.Count > 0)
                {
                    result = true;
                    writer.WriteLine(key);

                    foreach (SyntaxKind kind in kinds)
                    {
                        writer.WriteLine($"- {kind}");
                    }
                }
            }

            return result;
        }
    }
}