using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;

namespace Fasciculus.Eve.Models
{
    public readonly struct EveId : IEquatable<EveId>, IComparable<EveId>
    {
        public readonly int Value;

        public EveId(int value)
        {
            Value = value;
        }

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

        public override bool Equals([NotNullWhen(true)] object? obj)
            => obj is EveId id && Value == id.Value;

        public bool Equals(EveId other)
            => Value == other.Value;

        public int CompareTo(EveId other)
            => Value.CompareTo(other.Value);

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public override string? ToString()
        {
            return Value.ToString();
        }

        public static bool operator ==(EveId a, EveId b)
            => a.Value == b.Value;

        public static bool operator !=(EveId a, EveId b)
            => a.Value == b.Value;

        public static bool operator <(EveId a, EveId b)
            => a.Value < b.Value;

        public static bool operator >(EveId a, EveId b)
            => a.Value > b.Value;

        public static bool operator <=(EveId a, EveId b)
            => a.Value <= b.Value;

        public static bool operator >=(EveId a, EveId b)
            => a.Value >= b.Value;
    }

    public class EveObject
    {
        public EveId Id { get; }

        public int Index { get; internal set; }

        public EveObject(EveId id)
        {
            Id = id;
        }

        public virtual void Write(Stream stream)
        {
            Id.Write(stream);
        }

        protected static EveId BaseRead(Stream stream)
        {
            return EveId.Read(stream);
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

    public class EveNamedObject : EveObject
    {
        public string Name { get; }

        public EveNamedObject(EveId id, string name)
            : base(id)
        {
            Name = name;
        }

        public override void Write(Stream stream)
        {
            base.Write(stream);

            stream.WriteString(Name);
        }

        protected static new (EveId id, string name) BaseRead(Stream stream)
        {
            EveId id = EveObject.BaseRead(stream);
            string name = stream.ReadString();

            return (id, name);
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
