﻿using Fasciculus.Eve.Models;
using Fasciculus.Reflection;

namespace Fasciculus.Eve.Resources
{
    public static class EveResources
    {
        public static EveData ReadData()
            => EmbeddedResources.ReadCompressed("EveData", s => EveData.Read(s));

        public static EveUniverse ReadUniverse(EveNames names)
            => EmbeddedResources.ReadCompressed("EveUniverse", s => EveUniverse.Read(s, names));

        public static EveNavigation ReadNavigation(IEveUniverse universe)
            => EmbeddedResources.ReadCompressed("EveNavigation", s => EveNavigation.Read(universe, s));
    }
}
