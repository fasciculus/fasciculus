namespace Fasciculus.CodeAnalysis.Models
{
    /// <summary>
    /// Information of a package
    /// </summary>
    public class PackageInfo : ElementInfo
    {
        /// <summary>
        /// Merges this package with the given <paramref name="other"/> package.
        /// </summary>
        /// <param name="other"></param>
        public void Add(PackageInfo other)
        {
            Frameworks.Add(other.Frameworks);
        }
    }
}
