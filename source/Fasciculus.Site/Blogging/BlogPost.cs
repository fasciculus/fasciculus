using Statiq.Common;
using System;
using System.IO;

namespace Fasciculus.Site.Blogging
{
    public class BlogPost
    {
        public FileInfo File { get; }

        public NormalizedPath Source { get; }

        public IDocument? Document { get; set; } = null;

        public DateTime Created => GetCreated();

        public string Title => GetTitle();

        public string Link => GetLink();

        public BlogPost(FileInfo file)
        {
            File = file;
            Source = new(file.FullName, PathKind.Absolute);
        }

        private DateTime GetCreated()
        {
            if (Document is null) return File.CreationTime;

            if (Document["Created"] is string createdString)
            {
                if (DateTime.TryParse(createdString, out DateTime created))
                {
                    return created;
                }
            }

            return File.CreationTime;
        }

        private string GetTitle()
        {
            if (Document is null) return File.Name;

            return Document.GetTitle();
        }

        private string GetLink()
        {
            if (Document is null) return string.Empty;

            return Document.GetLink();
        }
    }
}
