using System;
using System.Threading.Tasks.Dataflow;

namespace Fasciculus.Utilities
{
    public abstract class TaskSafeProgress<T> : IProgress<T>
    {
        private readonly ActionBlock<T> action;

        protected TaskSafeProgress()
        {
            action = new(Process);
        }

        public void Report(T value)
        {
            action.Post(value);
        }

        protected abstract void Process(T value);
    }
}
