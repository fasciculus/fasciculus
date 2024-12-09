using Fasciculus.Support;
using System;

namespace Fasciculus.Maui.Support
{
    public enum WorkState
    {
        Pending,
        Working,
        Done
    }

    public class WorkStateProgress : TaskSafeProgress<WorkState>
    {
        public WorkStateProgress(Action<WorkState>? report = null)
            : base(report) { }
    }
}
