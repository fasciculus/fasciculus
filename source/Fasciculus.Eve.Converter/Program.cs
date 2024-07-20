using Fasciculus.Eve.Operations;
using System;

namespace Fasciculus.Eve
{
    public class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                ExtractSde.Extract();

                ConvertNames.Convert();
                ConvertRegions.Convert();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

        }
    }
}
