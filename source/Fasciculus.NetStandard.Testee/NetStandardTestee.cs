using System.Diagnostics.CodeAnalysis;

namespace Fasciculus.NetStandard.Testee
{
    public static class NetStandardTestee
    {
        public static string Foo1([DisallowNull] string bar)
        {
            return bar;
        }

        public static bool Foo2(string? input, [NotNullWhen(true)] out string? output)
        {
            output = input;

            return output is not null;
        }
    }
}
