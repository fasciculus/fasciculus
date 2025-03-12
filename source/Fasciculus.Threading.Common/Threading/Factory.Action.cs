using System;
using System.Threading.Tasks;

namespace Fasciculus.Threading
{
    /// <summary>
    /// Factory creating a task for an action.
    /// </summary>
    public interface IAsyncAction
    {
        /// <summary>
        /// Creates a task for the stored (async) action.
        /// </summary>
        public Task Create();
    }

    /// <summary>
    /// Factory creating a task for an action.
    /// </summary>
    public class AsyncAction : IAsyncAction
    {
        private readonly Action action;

        /// <summary>
        /// Initializes this factory.
        /// </summary>
        public AsyncAction(Action action)
        {
            this.action = action;
        }

        /// <summary>
        /// Creates a task for the stored action.
        /// </summary>
        public Task Create()
        {
            return Tasks.Start(action);
        }
    }

    /// <summary>
    /// Factory creating a task for an async action.
    /// </summary>
    public class AsyncWrappedAction : IAsyncAction
    {
        private readonly Func<Task> asyncAction;

        /// <summary>
        /// Initializes this factory.
        /// </summary>
        public AsyncWrappedAction(Func<Task> asyncAction)
        {
            this.asyncAction = asyncAction;
        }

        /// <summary>
        /// Creates a task for the stored async action.
        /// </summary>
        public Task Create()
        {
            return Tasks.Start(asyncAction).Unwrap();
        }
    }
}