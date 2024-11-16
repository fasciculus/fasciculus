using Fasciculus.Eve.Models;
using Fasciculus.Threading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fasciculus.Eve.Operations
{
    public class FindTradeOpportunities
    {
        private readonly Esi esi;
        private readonly EveTypes types;

        private readonly EveNpcStation origin;
        private readonly int originRegionId;

        private readonly EveNpcStation destination;
        private readonly int destinationRegionId;

        private readonly double volumePerType;
        private readonly double iskPerType;

        private readonly TaskSafeMutex mutex = new();
        private int typesCount;
        private int typesDone;
        private double bestMargin = -1;
        private readonly Action<string> progress;

        private FindTradeOpportunities(Esi esi, EveTypes types, EveNpcStation origin, EveNpcStation destination,
            double volumePerType, double iskPerType, Action<string> progress)
        {
            this.esi = esi;
            this.types = types;

            this.origin = origin;
            originRegionId = origin.Moon.Planet.SolarSystem.Constellation.Region.Id.Value;

            this.destination = destination;
            destinationRegionId = destination.Moon.Planet.SolarSystem.Constellation.Region.Id.Value;

            this.volumePerType = volumePerType;
            this.iskPerType = iskPerType;

            this.progress = progress;
        }

        public static async Task<TradeOpportunity[]> FindAsync(Esi esi, EveTypes types, EveNpcStation origin, EveNpcStation destination,
            double volumePerType, double iskPerType, Action<string> progress)
        {
            FindTradeOpportunities finder = new(esi, types, origin, destination, volumePerType, iskPerType, progress);

            return await finder.FindAsync();
        }

        private async Task<TradeOpportunity[]> FindAsync()
        {
            progress("finding trade opportunities");

            EveType[] canditateTypes = await FindCandidateTypes();

            typesCount = canditateTypes.Length;

            TradeOpportunity[] opportunities = canditateTypes.AsParallel().Select(FindOpportunity).AsEnumerable().NotNull().ToArray();

            esi.Flush();

            progress("finding trade opportunities done");

            return opportunities;
        }

        private TradeOpportunity? FindOpportunity(EveType type)
        {
            TradeOpportunity? opportunity = null;
            EveSupply? supply = FindSupply(type).Run();

            if (supply is not null)
            {
                EveDemand? demand = FindDemand(type).Run();

                if (demand is not null)
                {
                    opportunity = new(supply, demand, volumePerType, iskPerType);
                    OnOpportunity(opportunity);
                    opportunity = opportunity.Margin > 0.1 ? opportunity : null;
                }
            }

            OnTypeDone();

            return opportunity;
        }

        private async Task<EveSupply?> FindSupply(EveType type)
        {
            EsiMarketOrder[] orders = await esi.GetMarketOrdersAsync(originRegionId, type.Id.Value);
            EveSupply supply = EveSupply.Create(origin, type, orders, volumePerType, iskPerType);

            return supply.Valid ? supply : null;
        }

        private async Task<EveDemand?> FindDemand(EveType type)
        {
            EsiMarketOrder[] orders = await esi.GetMarketOrdersAsync(destinationRegionId, type.Id.Value);
            EveDemand demand = EveDemand.Create(destination, type, orders, volumePerType, iskPerType);

            return demand.Valid ? demand : null;
        }

        private async Task<EveType[]> FindCandidateTypes()
        {
            progress("  finding candidate types");

            EsiMarketPrice[] esiMarketPrices = await esi.GetMarketPricesAsync();
            EveMarketPrice[] eveMarketPrices = esiMarketPrices.Select(ConvertMarketPrice).ToArray();
            EveType[] candidateTypes = FindCandidateTypes(eveMarketPrices);

            progress("  finding candidate types done");

            return candidateTypes;
        }

        private EveType[] FindCandidateTypes(EveMarketPrice[] eveMarketPrices)
        {
            return eveMarketPrices
                .Where(mp => mp.AveragePrice <= iskPerType)
                .Where(mp => mp.Type.Volume <= volumePerType)
                .Select(mp => mp.Type)
                .ToArray();
        }

        private EveMarketPrice ConvertMarketPrice(EsiMarketPrice esiMarketPrice)
        {
            EveId id = EveId.Create(esiMarketPrice.TypeId);
            EveType type = types[id];

            return new(type, esiMarketPrice.AveragePrice);
        }

        private void OnTypeDone()
        {
            using Locker locker = Locker.Lock(mutex);

            ++typesDone;

            if (typesDone % 100 == 0)
            {
                esi.Flush();
                progress($"  {typesDone}/{typesCount}");
            }
        }

        private void OnOpportunity(TradeOpportunity? opportunity)
        {
            if (opportunity is null) return;

            using Locker locker = Locker.Lock(mutex);

            if (opportunity.Margin > bestMargin)
            {
                bestMargin = opportunity.Margin;
                progress($"  best margin {(bestMargin * 100):0.00} %");
            }
        }
    }
}
