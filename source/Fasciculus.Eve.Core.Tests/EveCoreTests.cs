using Fasciculus.Eve.Models;
using Fasciculus.Eve.Resources;

namespace Fasciculus.Eve.Core.Tests
{
    public class EveCoreTests : TestsBase
    {
        protected readonly static EveData data = EveResources.ReadData();

        protected readonly static EveUniverse universe = EveResources.ReadUniverse(data.Names);

        protected readonly static EveNavigation navigation = EveResources.ReadNavigation(universe);
    }
}
