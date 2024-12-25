using System.IO;

namespace Fasciculus.IO
{
    public class ResourceNotFoundException : IOException
    {
        public ResourceNotFoundException() { }

        public ResourceNotFoundException(string name) : base(name) { }
    }
}
