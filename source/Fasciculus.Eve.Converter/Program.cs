using System;
using System.IO;

namespace Fasciculus.Eve
{
    public class Program
    {
        public static FileInfo EveUniverseFile
            => Constants.ResourcesDirectory.File("EveUniverse.dat");

        public static void Main(string[] args)
        {
            try
            {
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

        }
    }
}
