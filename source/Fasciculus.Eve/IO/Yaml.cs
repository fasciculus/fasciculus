using System.IO;
using System.Text;
using YamlDotNet.RepresentationModel;

namespace Fasciculus.Eve.IO
{
    public static class Yaml
    {
        public static YamlDocument Load(string filename)
        {
            using Stream stream = new FileStream(filename, FileMode.Open, FileAccess.Read);
            using TextReader reader = new StreamReader(stream, Encoding.UTF8);
            YamlStream yaml = new();

            yaml.Load(reader);

            return yaml.Documents[0];
        }

        public static YamlDocument Load(FileInfo file)
            => Load(file.FullName);
    }
}
