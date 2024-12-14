namespace Fasciculus.Eve.Models
{
    public enum EveHauler : int
    {
        Shuttle = 10,
        Corvette = 100,
        Destroyer = 300,
        Fast = 2000,
        Bulk = 4800,
        DeepSpace = 50_000,
        Freighter = 400_000
    }

    public static class EveHaulers
    {
        public static EveHauler[] Values =>
        [
            EveHauler.Shuttle,
            EveHauler.Corvette,
            EveHauler.Destroyer,
            EveHauler.Fast,
            EveHauler.Bulk,
            EveHauler.DeepSpace,
            EveHauler.Freighter
        ];

        public static int Index(this EveHauler hauler)
        {
            return hauler switch
            {
                EveHauler.Shuttle => 0,
                EveHauler.Corvette => 1,
                EveHauler.Destroyer => 2,
                EveHauler.Fast => 3,
                EveHauler.Bulk => 4,
                EveHauler.DeepSpace => 5,
                EveHauler.Freighter => 6,
                _ => 0
            };
        }

        public static int Volume(this EveHauler hauler)
            => (int)hauler;

        public static EveHauler Parse(int volume)
        {
            if (volume > EveHauler.DeepSpace.Volume()) return EveHauler.Freighter;
            if (volume > EveHauler.Bulk.Volume()) return EveHauler.DeepSpace;
            if (volume > EveHauler.Fast.Volume()) return EveHauler.Bulk;
            if (volume > EveHauler.Destroyer.Volume()) return EveHauler.Fast;
            if (volume > EveHauler.Corvette.Volume()) return EveHauler.Destroyer;
            if (volume > EveHauler.Shuttle.Volume()) return EveHauler.Corvette;

            return EveHauler.Shuttle;
        }
    }
}
