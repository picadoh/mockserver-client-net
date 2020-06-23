using System.Threading.Tasks;

namespace MockServerClientNet.Extensions
{
    public static class TaskExtensions
    {
        public static T AwaitResult<T>(this Task<T> task)
        {
            return task.GetAwaiter().GetResult();
        }

        public static void AwaitResult(this Task task)
        {
            task.GetAwaiter().GetResult();
        }
    }
}