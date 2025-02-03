using System.Collections.Generic;

namespace Fasciculus.CodeAnalysis.Debugging
{
    public class NullCommentDebugger : ICommentDebugger
    {
        public void Handled(IEnumerable<string> tagNames) { }

        public void Used(string name) { }
    }
}
