using Fasciculus.CodeAnalysis.Frameworking;
using Microsoft.CodeAnalysis;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace Fasciculus.CodeAnalysis.Extensions
{
    /// <summary>
    /// Extensions for <see cref="Project"/>.
    /// </summary>
    public static class ProjectExtensions
    {
        public static FileInfo? GetFile(this Project project)
        {
            string? path = project.FilePath;

            if (!string.IsNullOrWhiteSpace(path))
            {
                FileInfo file = new FileInfo(path);

                return file.Exists ? file : null;
            }

            return null;
        }

        public static DirectoryInfo? GetDirectory(this Project project)
            => project.GetFile()?.Directory;

        /// <summary>
        /// Tries to parse the target framework of the given <paramref name="project"/>.
        /// </summary>
        public static bool TryGetTargetFramework(this Project project, [NotNullWhen(true)] out TargetFramework? framework)
        {
            framework = TargetFramework.UnsupportedFramework;

            string suffix = project.Name[project.AssemblyName.Length..];

            if (string.IsNullOrEmpty(suffix))
            {
                string? filePath = project.FilePath;

                if (!string.IsNullOrEmpty(filePath))
                {
                    XDocument document = XDocument.Load(filePath);
                    XElement? element = document.Descendants("TargetFramework").FirstOrDefault();

                    if (element is not null)
                    {
                        framework = TargetFramework.Parse(element.Value);

                        return true;
                    }
                }

                return false;
            }

            if (!suffix.StartsWith('(') || !suffix.EndsWith(')'))
            {
                return false;
            }

            framework = TargetFramework.Parse(suffix[1..^1]);

            return true;
        }

        public static TargetFramework GetTargetFramework(this Project project)
            => project.TryGetTargetFramework(out TargetFramework? framework) ? framework : TargetFramework.UnsupportedFramework;
    }
}
