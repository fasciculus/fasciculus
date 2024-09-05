using Fasciculus.Eve.Models;
using Fasciculus.Eve.Operations;
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
                ExtractSde.Extract();

                EveUniverseFile.Write(EveUniverse.Write);

                //Action[] actions =
                //{
                //    ConvertNames.Convert,
                //    ConvertUniverse.Convert
                //};

                //Task[] tasks = actions.Select(a => Task.Run(a)).ToArray();

                //Task.WaitAll(tasks);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

        }
    }
}
