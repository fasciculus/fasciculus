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
            data.WriteInt(objectsByIndex.Length);
            objectsByIndex.Apply(o => o.Write(data));
        }

        private IEnumerable<T> AsEnumerable()
            => objectsByIndex;

        public IEnumerator<T> GetEnumerator()
            => AsEnumerable().GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => AsEnumerable().GetEnumerator();
    }
}
