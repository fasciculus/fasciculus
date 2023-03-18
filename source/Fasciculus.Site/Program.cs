using Fasciculus.Site.Blogging;
using Statiq.App;
using Statiq.Common;
using Statiq.Web;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Fasciculus.Site
{
    public class Program
    {
        internal class PipelineEvent
        {
            public readonly string Name;
            public readonly Phase Phase;
            public readonly int Documents;

            public PipelineEvent(string name, Phase phase, int documents)
            {
                Name = name;
                Phase = phase;
                Documents = documents;
            }

            public override string ToString()
            {
                return $"{Name}/{Phase} ({Documents})";
            }
        }

        internal class ModuleEvent
        {
            public readonly string Pipeline;
            public readonly Phase Phase;
            public readonly string Module;
            public readonly int Documents;

            public ModuleEvent(AfterModuleExecution ev)
            {
                Pipeline = ev.Context.PipelineName;
                Phase = ev.Context.Phase;
                Module = ev.Context.Module.GetType().ToString();
                Documents = ev.Outputs.Length;
            }

            public override string ToString()
            {
                return $"{Pipeline}/{Phase}, {Module} ({Documents})";
            }
        }

        private static readonly List<PipelineEvent> _pipelineEvents = new();
        private static readonly List<ModuleEvent> _moduleEvents = new();

        public static async Task<int> Main(string[] args)
        {
            Bootstrapper bootstrapper = Bootstrapper
                .Factory
                .CreateWeb(args)
                .SetOutputPath(Locations.OutputDirectory.FullName);

            Blog.Initialize(bootstrapper);

            bootstrapper.SubscribeEvent<AfterPipelinePhaseExecution>(OnAfterPipelinePhaseExecution);
            bootstrapper.SubscribeEvent<AfterModuleExecution>(OnAfterModuleExecution);

            int result = await bootstrapper.RunAsync();

            WriteLine(string.Join("\n", _pipelineEvents));
            WriteLine("---------------------------------------------");
            WriteLine(string.Join("\n", _moduleEvents));

            return result;
        }

        private static void OnAfterPipelinePhaseExecution(AfterPipelinePhaseExecution ev)
        {
            _pipelineEvents.Add(new(ev.PipelineName, ev.Phase, ev.Outputs.Length));

            if (ev.PipelineName != "Content") return;
            if (ev.Phase != Phase.Process) return;
        }

        private static void OnAfterModuleExecution(AfterModuleExecution ev)
        {
            if (ev.Context.PipelineName != "Content") return;
            if (ev.Context.Phase != Phase.Process) return;

            _moduleEvents.Add(new(ev));
        }

        private static void WriteLine(string line)
        {
            Console.WriteLine(line);
            Debug.WriteLine(line);
        }
    }
}
