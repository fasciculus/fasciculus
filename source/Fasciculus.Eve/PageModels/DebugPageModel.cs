using Fasciculus.Eve.Models;
using Fasciculus.Eve.Services;
using Fasciculus.Maui.ComponentModel;
using Fasciculus.Threading;

namespace Fasciculus.Eve.PageModels
{
    public partial class DebugPageModel : MainThreadObservable
    {
        private const int SecondsPerDay = 24 * 60 * 60;

        public int BlueprintCount { get; private set; }
        public int BuyableBlueprints { get; private set; }
        public TimeSpan MinTime { get; private set; }
        public TimeSpan MaxTime { get; private set; }
        public int ShortBlueprints { get; private set; }
        public int LongBlueprints { get; private set; }

        public DebugPageModel(IEveProvider provider, IEsiClient esiClient, IPlanetChains planetChains)
        {
            EveMarketPrices? marketPrices = Tasks.Wait(esiClient.GetMarketPricesAsync());
            EveBlueprints blueprints = provider.Blueprints;

            BlueprintCount = blueprints.Count;

            if (marketPrices is not null)
            {
                EveBlueprint[] buyable = [.. blueprints.Where(x => marketPrices.Contains(x.Type))];

                BuyableBlueprints = buyable.Length;
                MinTime = TimeSpan.FromSeconds(buyable.Select(x => x.Manufacturing.Time).Min());
                MaxTime = TimeSpan.FromSeconds(buyable.Select(x => x.Manufacturing.Time).Max());
                ShortBlueprints = buyable.Count(x => x.Manufacturing.Time <= SecondsPerDay);
                LongBlueprints = buyable.Length - ShortBlueprints;
            }
        }
    }
}
