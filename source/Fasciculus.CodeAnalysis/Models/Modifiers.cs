using System.Collections.Generic;
using System.Diagnostics;

namespace Fasciculus.CodeAnalysis.Models
{
    /// <summary>
    /// Modifiers.
    /// </summary>
    public class Modifiers
    {
        /// <summary>
        /// Whether the containing object is 'public'.
        /// </summary>
        public bool IsPublic { get; private set; }

        /// <summary>
        /// Whether the containing object is 'static'.
        /// </summary>
        public bool IsStatic { get; private set; }

        /// <summary>
        /// Whether the containing object is 'abstract'.
        /// </summary>
        public bool IsAbstract { get; private set; }

        /// <summary>
        /// Whether the containing object is 'partial'.
        /// </summary>
        public bool IsPartial { get; private set; }

        /// <summary>
        /// Whether the containing object is accessible (public or protected).
        /// </summary>
        public bool IsAccessible => IsPublic;

        /// <summary>
        /// Initiaizes a mofifier colection.
        /// </summary>
        /// <param name="modifiers"></param>
        public Modifiers(IEnumerable<string> modifiers)
        {
            foreach (string modifier in modifiers)
            {
                switch (modifier)
                {
                    case "public": IsPublic = true; break;
                    case "static": IsStatic = true; break;
                    case "abstract": IsAbstract = true; break;
                    case "partial": IsPartial = true; break;

                    default:
                        Debug.WriteLine($"unhandled modifier '{modifier}'");
                        break;
                }
            }
        }
    }
}
