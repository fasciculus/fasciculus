using Fasciculus.Collections;
using Fasciculus.Threading.Synchronization;
using System.Collections.Generic;
using System.IO;

namespace Fasciculus.CodeAnalysis.Support
{
    public class CommentElementReporter
    {
        public static readonly CommentElementReporter Instance = new();

        private readonly TaskSafeMutex mutex = new();

        private readonly SortedSet<string> accepted
            = ["c", "code", "comment", "p", "para", "see", "summary", "typeparam"];

        private readonly SortedSet<string> used = [];

        private CommentElementReporter() { }

        public void Used(string name)
        {
            using Locker locker = Locker.Lock(mutex);

            used.Add(name);
        }

        public bool Report(StringWriter writer)
        {
            using Locker locker = Locker.Lock(mutex);

            bool result = false;
            SortedSet<string> notAccepted = new(used);

            notAccepted.ExceptWith(accepted);

            if (notAccepted.Count > 0)
            {
                result = true;

                notAccepted.Apply(s => { writer.WriteLine("- " + s); });
            }

            return result;
        }
    }
}
