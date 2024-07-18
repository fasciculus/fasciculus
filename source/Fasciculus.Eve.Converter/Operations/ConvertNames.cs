using Fasciculus.Eve.Models;
using Fasciculus.Eve.Utilities;
using System;
using System.Collections.Generic;
using System.IO;

namespace Fasciculus.Eve.Operations
{
    public static class ConvertNames
    {
        public static FileInfo SourceFile
            => Constants.BsdDirectory.File("invNames.yaml");

        public static FileInfo TargetFile
            => Constants.ResourcesDirectory.File("names.dat");

        public static void Run()
        {
            if (SourceFile.IsNewerThan(TargetFile))
            {
                Console.WriteLine("ConvertNames");
                Yaml.Deserialize<List<SdeName>>(SourceFile).ForEach(name => { Names.Set(name.itemID, name.itemName); });
                TargetFile.Write(Names.Save);
            }
        }
    }
}
