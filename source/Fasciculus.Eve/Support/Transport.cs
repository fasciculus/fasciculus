using Fasciculus.Eve.Models;

namespace Fasciculus.Eve.Support
{
    public static class EveHaulerExtensions
    {
        public static string Name(this EveHauler hauler)
        {
            return hauler switch
            {
                EveHauler.Shuttle => "Shuttle",
                EveHauler.Corvette => "Corvette",
                EveHauler.Destroyer => "Destroyer",
                EveHauler.Fast => "Fast Transport",
                EveHauler.Bulk => "Bulk Transport",
                EveHauler.DeepSpace => "Deep Space Transport",
                EveHauler.Freighter => "Freighter",
                _ => string.Empty
            };
        }

        public static string Caption(this EveHauler hauler)
        {
            string name = hauler.Name();
            string volume = hauler.Volume().ToString("#,###,##0", EveFormats.Volume);

            return $"{name} ({volume} m³)";
        }
    }
}
