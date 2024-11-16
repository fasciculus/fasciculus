using System;
using System.Collections.Generic;
using System.Linq;

namespace Fasciculus.Eve.Models
{
    public class EveOffer
    {
        private readonly List<EsiMarketOrder> orders;

        public EveNpcStation Station { get; }
        public EveType Type { get; }
        public int Quantity { get; }
        public bool Valid { get; }

        public double Price
            => orders.Count == 0 ? double.NaN : orders.Last().Price;

        public EveOffer(EveNpcStation station, EveType type, int quantity, bool valid, List<EsiMarketOrder> orders)
        {
            Station = station;
            Type = type;
            Quantity = quantity;
            Valid = valid;

            this.orders = orders;
        }

        protected static IEnumerable<EsiMarketOrder> Filter(EveNpcStation station, bool buyOrder, IEnumerable<EsiMarketOrder> orders)
        {
            long stationId = station.Id.Value;

            return orders.Where(o => o.IsBuyOrder == buyOrder && o.LocationId == stationId);
        }

        protected static List<EsiMarketOrder> Select(IEnumerable<EsiMarketOrder> orders, EveType type, double volumePerType, double iskPerType,
            out int quantity, out bool valid)
        {
            List<EsiMarketOrder> result = [];
            double volume = 0;
            double price = 0;

            quantity = 0;
            valid = false;

            foreach (var order in orders)
            {
                quantity += order.Quantity;
                volume += order.Quantity * type.Volume;
                price += order.Quantity * order.Price;

                result.Add(order);

                if (volume > volumePerType * 3 && price > iskPerType * 3)
                {
                    valid = true;
                    break;
                }
            }

            return result;
        }
    }

    public class EveSupply : EveOffer
    {
        public EveSupply(EveNpcStation station, EveType type, int quantity, bool valid, List<EsiMarketOrder> orders)
            : base(station, type, quantity, valid, orders) { }

        public static EveSupply Create(EveNpcStation station, EveType type, EsiMarketOrder[] orders, double volumePerType, double iskPerType)
        {
            IEnumerable<EsiMarketOrder> sorted = Filter(station, false, orders).OrderBy(o => o.Price);
            List<EsiMarketOrder> result = Select(sorted, type, volumePerType, iskPerType, out int quantity, out bool valid);

            return new(station, type, quantity, valid, result);
        }
    }

    public class EveDemand : EveOffer
    {
        public EveDemand(EveNpcStation station, EveType type, int quantity, bool valid, List<EsiMarketOrder> orders)
            : base(station, type, quantity, valid, orders) { }

        public static EveDemand Create(EveNpcStation station, EveType type, EsiMarketOrder[] orders, double volumePerType, double iskPerType)
        {
            IEnumerable<EsiMarketOrder> sorted = Filter(station, true, orders).OrderByDescending(o => o.Price);
            List<EsiMarketOrder> result = Select(sorted, type, volumePerType, iskPerType, out int quantity, out bool valid);

            return new(station, type, quantity, valid, result);
        }
    }

    public class TradeOpportunity
    {
        public EveSupply Supply { get; }
        public EveDemand Demand { get; }

        public double VolumePerType { get; }
        public double IskPerType { get; }

        public double BuyPrice => Supply.Price;
        public double SellPrice => Demand.Price;
        public double Margin => (SellPrice - BuyPrice) / BuyPrice;

        public int Quantity
        {
            get
            {
                int maxByVolume = (int)Math.Floor(VolumePerType / Supply.Type.Volume);
                int maxByPrice = (int)Math.Floor(IskPerType / BuyPrice);

                return Math.Min(maxByVolume, maxByPrice);
            }
        }

        public TradeOpportunity(EveSupply supply, EveDemand demand, double volumePerType, double iskPerType)
        {
            Supply = supply;
            Demand = demand;
            VolumePerType = volumePerType;
            IskPerType = iskPerType;
        }
    }
}
