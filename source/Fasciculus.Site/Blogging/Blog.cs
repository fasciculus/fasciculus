using Statiq.App;
using Statiq.Common;
using Statiq.Core;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Fasciculus.Site.Blogging
{
    public class Blog
    {
        public static DirectoryInfo SourceDirectory
            = new(Path.Combine(Locations.ProjectDirectory.FullName, "input", "blog"));

        private static readonly List<BlogPost> _posts = new();

        public static void Initialize(Bootstrapper bootstrapper)
        {
            foreach (FileInfo file in SourceDirectory.GetFiles("*.md", SearchOption.AllDirectories))
            {
                _posts.Add(new(file));
            }

            bootstrapper.SubscribeEvent<AfterModuleExecution>(OnAfterModuleExecution);
        }

        public static BlogPost[] Index()
        {
            return _posts.OrderByDescending(p => p.Created).Take(5).ToArray();
        }

        private static void OnAfterModuleExecution(AfterModuleExecution ev)
        {
            if (ev.Context.PipelineName != "Content") return;
            if (ev.Context.Phase != Phase.Process) return;
            if (ev.Context.Module.GetType() != typeof(SetDestination)) return;

            foreach (IDocument document in ev.Outputs)
            {
                UpdatePost(document);
            }
        }

        private static void UpdatePost(IDocument document)
        {
            BlogPost? post = _posts.FirstOrDefault(p => p.Source == document.Source);

            if (post != null)
            {
                post.Document = document;
            }
        }
    }
}
