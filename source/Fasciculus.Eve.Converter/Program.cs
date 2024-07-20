using Fasciculus.Eve.Operations;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Fasciculus.Eve
{
    public class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                ExtractSde.Extract();

                Action[] actions =
                {
                    ConvertNames.Convert,
                    ConvertUniverse.Convert
                };

                Task[] tasks = actions.Select(a => Task.Run(a)).ToArray();

                Task.WaitAll(tasks);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

        }
    }
}
