using Fasciculus.Maui.Collections;

namespace Fasciculus.Eve.Assets.Services
{
    public partial class ChangedResourcesSet : MainThreadSortedSet<string>
    {
        private readonly int prefix;

        public ChangedResourcesSet(IAssetsDirectories assetsDirectories)
        {
            prefix = assetsDirectories.Resources.FullName.Length + 1;
        }

        public void Add(FileInfo file)
            => base.Add(file.FullName[prefix..].Replace('\\', '/'));
    }
}
