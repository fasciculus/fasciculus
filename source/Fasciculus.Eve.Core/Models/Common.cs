using Fasciculus.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Fasciculus.Eve.Models
{
    public class EveId : Id<int>
    {
        public EveId(int value)
            : base(value) { }

        public static EveId Create(int id)
            => new(id);

        public void Write(Stream stream)
        {
            stream.WriteInt(Value);
        }

        public static EveId Read(Stream stream)
        {
            return new(stream.ReadInt());
        }
    }

    public class EveObject
    {
        public EveId Id { get; }

        public int Index { get; internal set; }

        public EveObject(EveId id)
        {
            Id = id;
        }
    }

    public class EveObjects<T> : IEnumerable<T>
        where T : notnull, EveObject
    {
        protected readonly T[] objectsByIndex;
        protected readonly Dictionary<EveId, T> objectsById;

        public int Count => objectsByIndex.Length;

        public IEnumerable<T> Objects
            => objectsByIndex;

        public EveObjects(T[] objects)
        {
            objectsByIndex = [.. objects.OrderBy(o => o.Id)];
            objectsById = objects.ToDictionary(o => o.Id);

            Enumerable.Range(0, objectsByIndex.Length).Apply(index => objectsByIndex[index].Index = index);
        }

        public T this[int index] => objectsByIndex[index];
        public T this[EveId id] => objectsById[id];

        public IEnumerator<T> GetEnumerator()
            => Objects.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => Objects.GetEnumerator();
    }

    public class EveNames
    {
        private readonly Dictionary<EveId, string> names;

        public EveNames(Dictionary<EveId, string> names)
        {
            this.names = names;
        }

        public string this[EveId id]
            => names[id];

        public void Write(Stream stream)
        {
            stream.WriteDictionary(names, k => k.Write(stream), v => stream.WriteString(v));
        }

        public static EveNames Read(Stream stream)
        {
            Dictionary<EveId, string> names = stream.ReadDictionary(_ => EveId.Read(stream), _ => stream.ReadString());

            return new(names);
        }
    }

    public class EveNamedObject : EveObject
    {
        private readonly EveNames names;

        public string Name
            => names[Id];

        public EveNamedObject(EveId id, EveNames names)
            : base(id)
        {
            this.names = names;
        }
    }

    public class EveNamedObjects<T> : EveObjects<T> where T : notnull, EveNamedObject
    {
        private readonly Dictionary<string, T> objectsByName;

        public EveNamedObjects(T[] objects)
            : base(objects)
        {
            objectsByName = objects.ToDictionary(o => o.Name);
        }

        public T this[string name]
            => objectsByName[name];
    }

    public class EveSecurity
    {
        public int Index { get; }
        public Func<EveSolarSystem, bool> Filter { get; }

        public EveSecurity(int index, Func<EveSolarSystem, bool> filter)
        {
            Index = index;
            Filter = filter;
        }

        public static readonly EveSecurity All
            = new(0, (ss) => true);

        public static readonly EveSecurity LowAndHigh
            = new(0, (ss) => ss.Security >= 0.0);

        public static readonly EveSecurity High
            = new(0, (ss) => ss.Security >= 0.5);

        public static IEnumerable<EveSecurity> Levels
            => [All, LowAndHigh, High];

        public static string Format(double security)
        {
            security = Math.Floor(security * 10.0) / 10.0;

            return $"{security:0.0}";
        }
    }
}
