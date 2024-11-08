using Fasciculus.Eve.Models;
using Fasciculus.Reflection;

namespace Fasciculus.Eve.Resources
{
    public static class EveResources
    {
        public static EveUniverse ReadUniverse()
            => EmbeddedResources.Read("EveUniverse", s => EveUniverse.Read(s));

        public static EveNavigation ReadNavigation(EveSolarSystems solarSystems)
            => EmbeddedResources.ReadCompressed("EveNavigation", s => EveNavigation.Read(solarSystems, s));
    }
}
