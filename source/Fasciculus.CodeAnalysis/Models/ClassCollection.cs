using Fasciculus.CodeAnalysis.Frameworking;
using Fasciculus.Collections;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Fasciculus.CodeAnalysis.Models
{
    /// <summary>
    /// Collection of classes.
    /// </summary>
    public class ClassCollection : IElementCollection<ClassInfo>
    {
        private readonly Dictionary<string, ClassInfo> classes = [];

        /// <summary>
        /// Adds a new class with the given <paramref name="name"/> or returns an existing class with that name.
        /// </summary>
        public ClassInfo Add(string name, string untypedName, IEnumerable<string> parameters, Modifiers modifiers)
        {
            if (classes.TryGetValue(name, out ClassInfo? existing))
            {
                return existing;
            }

            ClassInfo @class = new(name, untypedName, parameters, modifiers);

            return MergeWith(@class);
        }

        /// <summary>
        /// Adds the given <paramref name="class"/> to this collection, merging it into an existing class of the
        /// same name if such one exists.
        /// </summary>
        public ClassInfo MergeWith(ClassInfo @class)
        {
            if (classes.TryGetValue(@class.Name, out ClassInfo? existing))
            {
                existing.MergeWith(@class);

                return existing;
            }
            else
            {
                classes[@class.Name] = @class;

                return @class;
            }
        }

        /// <summary>
        /// Adds the given <paramref name="framework"/>
        /// </summary>
        public void AddFramework(TargetFramework framework)
            => classes.Values.Apply(n => { n.AddFramework(framework); });

        /// <summary>
        /// Adds the given <paramref name="name"/> to the packages this namespaces occurs in.
        /// </summary>
        public void AddPackage(string name)
            => classes.Values.Apply(c => { c.AddPackage(name); });

        /// <summary>
        /// Returns an enumerator that iterates through this collection.
        /// </summary>
        public IEnumerator<ClassInfo> GetEnumerator()
            => classes.Values.OrderBy(x => x.Name).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => classes.Values.OrderBy(x => x.Name).GetEnumerator();
    }
}
