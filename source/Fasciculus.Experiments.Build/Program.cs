using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace Fasciculus.Experiments.Build
{
    internal class Program
    {
        static void Main(string[] args)
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

                lines.Add($"export const {key} = {value};");
            }

            string profilerSession = Guid.NewGuid().ToString("N");

            lines.Add($"export const PROFILER_SESSION = \"{profilerSession}\";");

            File.WriteAllLines("Config.ts", lines);
        }
    }
}
