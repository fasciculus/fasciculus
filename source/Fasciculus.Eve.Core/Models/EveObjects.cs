using Fasciculus.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Fasciculus.Eve.Models
{
    public class EveObjects<T> : IEnumerable<T>
        where T : notnull, EveObject
    {
        protected readonly T[] objectsByIndex;
        protected readonly Dictionary<EveId, T> objectsById;

        public int Count => objectsByIndex.Length;

        public IEnumerable<T> Objects
            => objectsByIndex;

        public EveObjects(IEnumerable<T> objects)
        {
            objectsByIndex = [.. objects.OrderBy(o => o.Id)];
            objectsById = objects.ToDictionary(o => o.Id);

            Enumerable.Range(0, objectsByIndex.Length).Apply(index => objectsByIndex[index].Index = index);
        }

        public T this[int index] => objectsByIndex[index];
        public T this[EveId id] => objectsById[id];

        public void Write(Data data)
        {
            data.WriteArray(objectsByIndex, o => o.Write(data));
        }

        public IEnumerator<T> GetEnumerator()
            => Objects.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => Objects.GetEnumerator();
    }
}
