using Fasciculus.Eve.Models;
using Fasciculus.Eve.Operations;
using Fasciculus.IO;
using System;
using System.IO;

namespace Fasciculus.Eve
{
    public class Program
    {
        public static FileInfo EveDataFile
            => Constants.ResourcesDirectory.File("EveData.dat");

        public static void Main(string[] args)
        {
            try
            {
                ExtractSde.Extract();

                EveData eveData = new EveData();

                EveDataFile.Write(stream => eveData.Write(new Data(stream)));

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
