using Fasciculus.Mathematics;

namespace Fasciculus.Eve.Models
{
    public class EveDistances
    {
        private readonly IMatrix<int> distances;

        private EveDistances(IMatrix<int> distances)
        {
            this.distances = distances;
        }

        public static EveDistances Create(IEveUniverse universe, double minSecurity)
        {
            IMutableMatrix<int> distances = InitializeDistances(universe);

            return new(distances);
        }

        private static IMutableMatrix<int> InitializeDistances(IEveUniverse universe)
        {
            int count = universe.SolarSystems.Count;

            IMutableMatrix<int> distances = Matrices.CreateMutableDenseInt(count, count);

            for (int row = 0; row < count; ++row)
            {
                for (int col = 0; col < count; ++col)
                {
                    distances.Set(row, col, row == col ? 0 : int.MaxValue);
                }
            }

            return distances;
        }
    }
}
