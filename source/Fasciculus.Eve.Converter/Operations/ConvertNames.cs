using Fasciculus.Eve.Models;
using Fasciculus.Eve.Models.Sde;
using Fasciculus.Eve.Utilities;
using Fasciculus.IO;
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
            => Constants.ResourcesDirectory.File("Names.dat");

        public static void Convert()
        {
            if (SourceFile.IsNewerThan(TargetFile))
            {
                Console.WriteLine("ConvertNames");
                Yaml.Deserialize<List<SdeName>>(SourceFile).ForEach(name => { Names.Set(name.itemID, name.itemName); });
                TargetFile.Write(stream => Names.Write(new Data(stream)));
            }
        }
    }
}
