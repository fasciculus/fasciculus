using System.Collections.Generic;
using System.Linq;

namespace Fasciculus.Eve.Models
{
    public class EveNamedObjects<T> : EveObjects<T> where T : notnull, EveNamedObject
    {
        private readonly Dictionary<string, T> objectsByName;

        public EveNamedObjects(IEnumerable<T> objects)
            : base(objects)
        {
            objectsByName = objects.ToDictionary(o => o.Name);
        }

        public T this[string name]
            => objectsByName[name];
    }
}
