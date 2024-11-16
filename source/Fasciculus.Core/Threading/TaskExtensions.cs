namespace System.Threading.Tasks
{
    public static class TaskExtensions
    {
        public static T Run<T>(this Task<T> task)
            => task.GetAwaiter().GetResult();
    }
}
