using Fasciculus.IO.Resources;
using Fasciculus.Support;
using System;
using System.Linq;
using System.Reflection;

namespace Fasciculus.IO
{
    public interface IEmbeddedResources
    {
        public EmbeddedResource this[string name] { get; }
    }

    public class EmbeddedResources : IEmbeddedResources
    {
        public EmbeddedResource this[string name] => Find(name);

        private EmbeddedResource Find(string name)
        {
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                string[] names = assembly.GetManifestResourceNames();

                if (names.Contains(name))
                {
                    return new EmbeddedResource(assembly, name);
                }
            }

            throw Ex.ResourceNotFound(name);
        }
    }
}
