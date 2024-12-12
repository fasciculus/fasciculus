using Fasciculus.Eve.Models;
using Fasciculus.Threading;

namespace Fasciculus.Eve.Services
{
    public interface IMarket
    {
        public EveStation Hub { get; }
    }

    public class Market : IMarket
    {
        public EveStation Hub { get; }

        public Market(IEveResources resources)
        {
            Hub = Tasks.Wait(resources.Universe).Stations[60003760];
        }
    }
}
