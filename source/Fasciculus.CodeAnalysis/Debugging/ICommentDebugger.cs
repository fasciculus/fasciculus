using System.Collections.Generic;

namespace Fasciculus.CodeAnalysis.Debugging
{
    public interface ICommentDebugger
    {
        public void Handled(IEnumerable<string> tagNames);

        public void Used(string name);
    }
}
