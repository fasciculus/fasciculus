using Fasciculus.Collections;
using Fasciculus.Eve.Models;
using System.Collections.Generic;
using System.Linq;

namespace Fasciculus.Eve.Services
{
    public interface IPlanetChains
    {
        public EvePlanetChain[] this[int outputLevel, int inputLevel] { get; }
    }

    public class PlanetChains : IPlanetChains
    {
        private readonly IPlanetSchematics schematics;

        private readonly Dictionary<int, Dictionary<int, List<EvePlanetChain>>> chains;

        public EvePlanetChain[] this[int outputLevel, int inputLevel] => GetChains(outputLevel, inputLevel);

        public PlanetChains(IPlanetSchematics schematics)
        {
            this.schematics = schematics;
            chains = [];

            AddChains();
        }

        private EvePlanetChain[] GetChains(int outputLevel, int inputLevel)
        {
            if (chains.TryGetValue(outputLevel, out Dictionary<int, List<EvePlanetChain>>? byInput))
            {
                return byInput.TryGetValue(inputLevel, out List<EvePlanetChain>? result) ? [.. result] : [];
            }

            return [];
        }

        private void AddChains()
        {
            AddChains(schematics.P2, 1);
            AddChains(schematics.P3, 1);
            AddChains(schematics.P3, 2);
            AddChains(schematics.P4, 1);
            AddChains(schematics.P4, 2);
            AddChains(schematics.P4, 3);
        }

        private void AddChains(IEnumerable<EvePlanetSchematic> outputs, int inputLevel)
        {
            outputs.Apply(x => { AddChain(x, inputLevel); });
        }

        private void AddChain(EvePlanetSchematic output, int inputLevel)
        {
            EvePlanetTypeQuantity[] inputs = CollectInputs(output, inputLevel, 1);
            EvePlanetChain chain = new(inputs, output.Output, inputLevel, output.Level);

            AddChain(chain);
        }

        private EvePlanetTypeQuantity[] CollectInputs(EvePlanetSchematic output, int inputLevel, int runs)
        {
            EvePlanetTypeQuantity[] all = [.. output.Inputs];
            EvePlanetTypeQuantity[] resolveds = [.. all.Where(x => schematics[x.Type].Level == inputLevel)];
            EvePlanetTypeQuantity[] unresolveds = [.. all.Where(x => schematics[x.Type].Level != inputLevel)];
            EvePlanetTypeQuantity[] result = [.. resolveds.Select(x => new EvePlanetTypeQuantity(x.Type, runs * x.Quantity))];

            foreach (EvePlanetTypeQuantity unresolved in unresolveds)
            {
                EvePlanetSchematic schematic = schematics[unresolved.Type];
                int requiredQuantity = unresolved.Quantity;
                int providedQuantity = schematic.Output.Quantity;
                int runs2 = runs * (requiredQuantity + providedQuantity - 1) / providedQuantity;

                result = [.. result.Concat(CollectInputs(schematic, inputLevel, runs2))];
            }

            return result;
        }

        private void AddChain(EvePlanetChain chain)
        {
            int outputLevel = chain.OutputLevel;
            int inputLevel = chain.InputLevel;

            if (!chains.TryGetValue(outputLevel, out Dictionary<int, List<EvePlanetChain>>? outputChains))
            {
                outputChains = [];
                chains[outputLevel] = outputChains;
            }

            if (!outputChains.TryGetValue(inputLevel, out List<EvePlanetChain>? inputChains))
            {
                inputChains = [];
                outputChains[inputLevel] = inputChains;
            }

            inputChains.Add(chain);
        }
    }
}
