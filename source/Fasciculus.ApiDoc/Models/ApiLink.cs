using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Fasciculus.ApiDoc.Models
{
    public class ApiLink : IEnumerable<string>
    {
        private readonly List<string> parts;

        public IReadOnlyList<string> Parts => parts;

        public ApiLink(params string[] parts)
        {
            this.parts = new(parts);
        }

        private ApiLink(ApiLink other, string part)
        {
            parts = new(other.Append(part));
        }

        public ApiLink Combine(string part, int parameterCount = 0)
            => new(this, parameterCount > 0 ? $"{part}-{parameterCount}" : part);

        public IEnumerator<string> GetEnumerator()
            => parts.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => parts.GetEnumerator();

        public override string? ToString()
        {
            return string.Join('/', parts);
        }
    }
}
