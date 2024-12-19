using System.Collections.Generic;

namespace Fasciculus.Eve.Models
{
    public class EvePlanetChain
    {
        private readonly EvePlanetTypeQuantity[] inputs;

        public IReadOnlyList<EvePlanetTypeQuantity> Inputs => inputs;
        public EvePlanetTypeQuantity Output;

        public int InputLevel { get; }
        public int OutputLevel { get; }

        internal EvePlanetChain(EvePlanetTypeQuantity[] inputs, EvePlanetTypeQuantity output, int inputLevel, int outputLevel)
        {
            this.inputs = inputs;

            Output = output;

            InputLevel = inputLevel;
            OutputLevel = outputLevel;
        }
    }
}
