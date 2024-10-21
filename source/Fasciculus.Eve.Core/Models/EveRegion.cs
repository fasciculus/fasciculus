namespace Fasciculus.Eve.Models
{
    public class EveRegion : EveNamedObject
    {
        public int Index { get; internal set; }

        public EveRegion(EveId id, string name)
            : base(id, name) { }
    }
}
