namespace Fasciculus.Models
{
    public class Pair<T1, T2>
        where T1 : notnull
        where T2 : notnull
    {
        public readonly T1 First;
        public readonly T2 Second;

        public Pair(T1 first, T2 second)
        {
            First = first;
            Second = second;
        }

        public override int GetHashCode()
            => First.GetHashCode() + Second.GetHashCode();

        public override string ToString()
            => $"({First}, {Second})";
    }
}
