using System.Globalization;

namespace Fasciculus.Eve.Support
{
    public static class EveFormats
    {
        public static readonly NumberFormatInfo Isk = CreateIsk();

        private static NumberFormatInfo CreateIsk()
        {
            CultureInfo ci = CultureInfo.GetCultureInfo("en-US");
            NumberFormatInfo nf = (NumberFormatInfo)ci.NumberFormat.Clone();

            nf.NumberGroupSeparator = "'";

            return nf;
        }
    }
}
