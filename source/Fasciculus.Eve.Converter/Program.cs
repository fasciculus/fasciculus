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
                Console.WriteLine("ExtractSde");
                ExtractSde.Run();

                ConvertNames.Run();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

        }
    }
}
