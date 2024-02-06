using Microsoft.Build.Utilities;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace Fasciculus.Experiments.Build
{
    public class CreateConfig : Task
    {
        public override bool Execute()
        {
            XmlDocument doc = new XmlDocument();

            doc.Load("Config.xml");

            XmlNodeList adds = doc.GetElementsByTagName("Add");
            List<string> lines = new List<string>();

            foreach (var add in adds)
            {
                XmlElement element = (XmlElement)add;
                string key = element.GetAttribute("key");
                string value = element.InnerText;
                lines.Add("export const " + key + " = " + value + ";");
            }

            File.WriteAllLines("ts/Config.ts", lines);

            return true;
        }
    }
}
