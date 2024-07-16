namespace System.IO
{
    public static class DirectoryInfoExtensions
    {
        public static FileInfo File(this DirectoryInfo directory, string name)
            => new(Path.Combine(directory.FullName, name));

        public static DirectoryInfo Existing(this DirectoryInfo directory, bool create = true)
        {
            DirectoryInfo result = new(directory.FullName);

            if (!result.Exists)
            {
                if (create)
                {
                    result.Create();
                    result = new(directory.FullName);
                }
                else
                {
                    throw new DirectoryNotFoundException(directory.FullName);
                }
            }

            return result;
        }

        public static DirectoryInfo Combine(this DirectoryInfo directory, params string[] paths)
        {
            string result = directory.FullName;

            foreach (string path in paths)
            {
                result = Path.Combine(result, path);
            }

            return new DirectoryInfo(result);
        }
    }
}
