namespace Fasciculus.Eve.Models
{
    public class EveOffer
    {
        public EveType Type { get; }
        public EveNpcStation Station { get; }

        public EveOffer(EveType type, EveNpcStation station)
        {
            Type = type;
            Station = station;
        }
    }

    public class EveSupply : EveOffer
    {
        public EveSupply(EveType type, EveNpcStation station)
            : base(type, station) { }
    }

    public class EveDemand : EveOffer
    {
        public EveDemand(EveType type, EveNpcStation station)
            : base(type, station) { }
    }

    public class TradeOpportunity
    {
        public EveSupply Supply { get; }
        public EveDemand Demand { get; }

        public TradeOpportunity(EveSupply supply, EveDemand demand)
        {
            Supply = supply;
            Demand = demand;
        }
    }

    public class TradeOpportunities
    {
        public static void Create(EveTypes types, EveNpcStation origin, EveNpcStation destination, double volumePerType, double iskPerType)
        {
        }
    }
}
