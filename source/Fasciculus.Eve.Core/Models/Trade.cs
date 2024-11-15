using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

                if (volume > volumePerType * 10 && price > iskPerType * 10)
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

    public class TradeOpportunities
    {
        public static async Task<TradeOpportunity[]> CreateAsync(Esi esi, EveTypes types, EveNpcStation origin, EveNpcStation destination,
            double volumePerType, double iskPerType)
        {
            Console.WriteLine($"Searching opportunities");

            int originRegion = origin.Moon.Planet.SolarSystem.Constellation.Region.Id.Value;
            int destinationRegion = destination.Moon.Planet.SolarSystem.Constellation.Region.Id.Value;
            EsiMarketPrice[] esiMarketPrices = await esi.GetMarketPricesAsync();
            EveMarketPrice[] eveMarketPrices = esiMarketPrices.Select(mp => ConvertMarketPrice(types, mp)).ToArray();
            EveType[] candidates = FilterMarketPrices(eveMarketPrices, volumePerType, iskPerType);
            List<TradeOpportunity> opportunities = [];
            int progress = 0;
            double bestMargin = -1;

            foreach (EveType type in candidates)
            {
                ++progress;

                if ((progress % 50) == 0)
                {
                    Console.WriteLine($"  progress {progress}/{candidates.Length}");
                }

                EsiMarketOrder[] originOrders = await esi.GetMarketOrdersAsync(originRegion, type.Id.Value);
                EveSupply supply = EveSupply.Create(origin, type, originOrders, volumePerType, iskPerType);

                if (!supply.Valid) continue;

                EsiMarketOrder[] destinationOrders = await esi.GetMarketOrdersAsync(destinationRegion, type.Id.Value);
                EveDemand demand = EveDemand.Create(destination, type, destinationOrders, volumePerType, iskPerType);

                if (!demand.Valid) continue;

                TradeOpportunity opportunity = new(supply, demand, volumePerType, iskPerType);

                if (opportunity.Margin > bestMargin)
                {
                    bestMargin = opportunity.Margin;
                    Console.WriteLine($"  best margin {(bestMargin * 100):0.0} %");
                }

                if (opportunity.Margin > 0.1)
                {
                    opportunities.Add(opportunity);

                    if (opportunities.Count > 9) break;
                }
            }

            return opportunities.ToArray();
        }

        private static EveType[] FilterMarketPrices(EveMarketPrice[] eveMarketPrices, double volumePerType, double iskPerType)
        {
            return eveMarketPrices
                .Where(mp => mp.AveragePrice <= iskPerType)
                .Where(mp => mp.Type.Volume <= volumePerType)
                .Select(mp => mp.Type)
                .ToArray();
        }

        private static EveMarketPrice ConvertMarketPrice(EveTypes types, EsiMarketPrice esiMarketPrice)
        {
            EveId id = EveId.Create(esiMarketPrice.TypeId);
            EveType type = types[id];

            return new(type, esiMarketPrice.AveragePrice);
        }
    }
}
