using Microsoft.Build.Utilities;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace Fasciculus.Screeps.Bot
{
    public class CreateConfig : Task
    {
        public override bool Execute()
        {
            XmlDocument doc = new XmlDocument();

            doc.Load("config.xml");

            XmlNodeList adds = doc.GetElementsByTagName("Add");
            List<string> lines = new List<string>();

            foreach (var add in adds)
            {
                XmlElement element = (XmlElement)add;
                string key = element.GetAttribute("key");
                string type = element.GetAttribute("type");
                string value = element.InnerText;

                lines.Add($"export const {key}: {type} = {value}" + ";");
            }

            File.WriteAllLines("ts/config.ts", lines);

            return true;
        }
    }
}
