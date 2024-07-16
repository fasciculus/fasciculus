using Fasciculus.Eve.IO;
using Fasciculus.Eve.Models;
using Fasciculus.Eve.Models.Sde;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using YamlDotNet.RepresentationModel;

namespace Fasciculus.Eve.Actions
{
    public static class LoadNames
    {
        private static readonly YamlScalarNode idKey = new YamlScalarNode("itemID");
        private static readonly YamlScalarNode nameKey = new YamlScalarNode("itemName");

        public static async Task LoadAsync()
        {
            FileInfo file = Constants.BsdDirectory.File("invNames.yaml");
            List<SdeName> names = await Yaml.DeserializeAsync<List<SdeName>>(file);

            foreach (var name in names)
            {
                Names.Set(name.itemID, name.itemName);
            }
        }
    }
}
