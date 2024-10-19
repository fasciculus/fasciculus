﻿using Fasciculus.Eve.Models;
using Fasciculus.Eve.Operations;
using System;
using System.Threading.Tasks;

namespace Fasciculus.Eve
{
    public class Program
    {
        private class Progress : IProgress<string>
        {
            public void Report(string message)
            {
                Console.WriteLine(message);
            }
        }

        public static async Task Main(string[] args)
        {
            try
            {
                Progress progress = new();

                await DownloadSdeZip.Execute(progress);
                await ExtractSdeZip.Execute(progress);

                Task<SdeUniverse> parseUniverse = Task.Run(() => ParseUniverse.Execute(progress));
                Task<SdeData> parseData = Task.Run(() => ParseData.Execute(progress));

                Task[] tasks = [parseUniverse, parseData];

                tasks.WaitAll();

                SdeUniverse universe = parseUniverse.Result;
                SdeData data = parseData.Result;

                universe.Populate(data);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    }
}
