namespace System.Threading.Tasks
{
    public static class TaskExtensions
    {
        public static T Run<T>(this Task<T> task)
        {
            Task.WaitAll([task]);

            return task.Result;
        }
    }
}
