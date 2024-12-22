using Fasciculus.Eve.Models;
using Fasciculus.Threading.Synchronization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace Fasciculus.Eve.Services
{
    public partial class EsiClient : IEsiClient
    {
        public async Task<EveIndustryIndices?> GetIndustryIndicesAsync()
        {
            using Locker locker = Locker.Lock(mutex);

            EveIndustryIndices.Data? data = esiCache.GetIndustryIndices();

            if (data is null)
            {
                string? text = await esiHttp.GetSingle("industry/systems/");

                if (text is not null)
                {
                    EsiIndustrySystem[] systems = JsonSerializer.Deserialize<EsiIndustrySystem[]>(text) ?? [];

                    if (systems.Length > 0)
                    {
                        data = ConvertIndustrySystems(systems);
                        esiCache.SetIndustryIndices(data);
                    }
                }
            }

            return data is null ? null : new(data);
        }

        private EveIndustryIndices.Data ConvertIndustrySystems(EsiIndustrySystem[] systems)
        {
            Dictionary<uint, double> data = systems
                .Where(x => solarSystems.Contains(x.SolarSystem))
                .Select(x => Tuple.Create(x.SolarSystem, ConvertIndustryCostIndices(x.CostIndices)))
                .ToDictionary();

            return new(data);
        }

        private double ConvertIndustryCostIndices(EsiIndustryCostIndex[] costIndices)
            => costIndices.FirstOrDefault(x => "manufacturing" == x.Activity)?.CostIndex ?? 0;
    }
}
