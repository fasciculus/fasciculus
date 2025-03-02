using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace Fasciculus.Threading.Experiment
{
    internal class CustomTaskScheduler : TaskScheduler
    {
        private readonly TaskScheduler target;

        private readonly MethodInfo getScheduledTasks;
        private readonly MethodInfo queueTask;
        private readonly MethodInfo tryDequeue;
        private readonly MethodInfo tryExecuteTaskInline;

        public override int MaximumConcurrencyLevel => target.MaximumConcurrencyLevel;

        public CustomTaskScheduler(TaskScheduler target)
        {
            this.target = target;

            Type type = target.GetType();

            getScheduledTasks = type.GetMethod("GetScheduledTasks", BindingFlags.NonPublic | BindingFlags.Instance) ?? throw new InvalidOperationException();
            queueTask = type.GetMethod("QueueTask", BindingFlags.NonPublic | BindingFlags.Instance) ?? throw new InvalidOperationException();
            tryDequeue = type.GetMethod("TryDequeue", BindingFlags.NonPublic | BindingFlags.Instance) ?? throw new InvalidOperationException();
            tryExecuteTaskInline = type.GetMethod("TryExecuteTaskInline", BindingFlags.NonPublic | BindingFlags.Instance) ?? throw new InvalidOperationException();
        }

        protected override IEnumerable<Task>? GetScheduledTasks()
        {
            return (IEnumerable<Task>?)getScheduledTasks.Invoke(target, []);
        }

        protected override void QueueTask(Task task)
        {
            Logger.Log($"QueueTask {task.Id}");

            queueTask.Invoke(target, [task]);
        }

        protected override bool TryDequeue(Task task)
        {
            Logger.Log($"TryDequeue {task.Id}");

            return (bool)tryDequeue.Invoke(target, [task])!;
        }

        protected override bool TryExecuteTaskInline(Task task, bool taskWasPreviouslyQueued)
        {
            Logger.Log($"TryExecuteTaskInline {task.Id}");

            return (bool)(tryExecuteTaskInline.Invoke(target, [task, taskWasPreviouslyQueued]))!;
        }
    }
}
