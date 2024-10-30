﻿using Fasciculus.Eve.Models;
using Fasciculus.Reflection;

namespace Fasciculus.Eve.Core.Tests
{
    public class EveCoreTests
    {
        protected readonly IEveUniverse universe = EmbeddedResources.Read("EveUniverse", EveUniverse.Read);
    }
}