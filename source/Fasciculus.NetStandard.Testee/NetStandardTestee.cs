using System.Diagnostics.CodeAnalysis;

namespace Fasciculus.NetStandard.Testee
{
    public static class NetStandardTestee
    {
        public static string Foo([DisallowNull] string bar)
        {
            return bar;
        }
    }
}
