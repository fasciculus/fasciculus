namespace Fasciculus.IO
{
    /// <summary>
    /// Whether to overwrite a target file. Used with
    /// <see cref="FileInfoExtensions.RequiresOverwrite(System.IO.FileInfo, System.DateTime, FileOverwriteMode)"/>
    /// </summary>
    public enum FileOverwriteMode
    {
        /// <summary>
        /// Never overwrite an existing file.
        /// </summary>
        Never,

        /// <summary>
        /// Overwrite if the file doesn't exist or if the file's last write time is older than a given time.
        /// </summary>
        IfNewer,

        /// <summary>
        /// Always overwrite the file.
        /// </summary>
        Always
    }
}
