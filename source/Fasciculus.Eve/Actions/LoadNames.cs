using Fasciculus.Eve.IO;
using Fasciculus.Eve.Models;
using Fasciculus.Eve.Models.Sde;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace Fasciculus.Eve.Actions
{
    public static class LoadNames
    {
        public static async Task LoadAsync()
        {
            Stopwatch stopwatch = new Stopwatch();

            stopwatch.Start();

            FileInfo file = Constants.BsdDirectory.File("invNames.yaml");
            List<SdeName> names = await Yaml.DeserializeAsync<List<SdeName>>(file);

            foreach (var name in names)
            {
                Names.Set(name.itemID, name.itemName);
            }

            Console.WriteLine("LoadNames: " + stopwatch.ElapsedMilliseconds);
        }
    }
}
