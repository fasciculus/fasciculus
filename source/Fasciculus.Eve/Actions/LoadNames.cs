using Fasciculus.Eve.IO;
using Fasciculus.Eve.Models;
using System.IO;
using System.Threading.Tasks;
using YamlDotNet.RepresentationModel;

namespace Fasciculus.Eve.Actions
{
    public static class LoadNames
    {
        private static readonly YamlScalarNode idKey = new YamlScalarNode("itemID");
        private static readonly YamlScalarNode nameKey = new YamlScalarNode("itemName");

        public static async Task RunAsync()
        {
            YamlDocument document = Yaml.Load(Constants.BsdDirectory.File("invNames.yaml"));
            YamlSequenceNode root = (YamlSequenceNode)document.RootNode;

            foreach (YamlMappingNode entry in root.Children)
            {
                string? id = entry.Children[idKey]?.ToString();
                string? name = entry.Children[nameKey]?.ToString();

                if (id is null) continue;
                if (name is null) continue;

                Names.Set(int.Parse(id), name);
            }

            await Task.Delay(0);
        }
    }
}
