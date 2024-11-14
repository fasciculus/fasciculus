namespace Fasciculus.Eve.Models
{
    public class EveName : EveNamedObject
    {
        public EveName(EveId id, string name)
            : base(id, name) { }
    }

    public class EveNames : EveNamedObjects<EveName>
    {
        public EveNames(EveName[] names)
            : base(names) { }
    }

    public class EveData
    {
        public EveNames Names { get; }

        public EveData(EveNames names)
        {
            Names = names;
        }
    }
}
