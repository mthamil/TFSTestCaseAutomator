using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace xUnit.AutomationProvider
{
    static class Extensions
    {
        public static void ToSink<T>(this IEnumerable<T> source, ICollection<T> destination)
        {
            foreach (var item in source)
                destination.Add(item);
        }

        /// <remarks>See http://stackoverflow.com/questions/18756354/wrapping-manualresetevent-as-awaitable-task</remarks>
        public static Task AsTask(this WaitHandle handle)
        {
            return AsTask(handle, Timeout.InfiniteTimeSpan);
        }

        /// <remarks>See http://stackoverflow.com/questions/18756354/wrapping-manualresetevent-as-awaitable-task</remarks>
        public static Task AsTask(this WaitHandle handle, TimeSpan timeout)
        {
            var tcs = new TaskCompletionSource<object>();
            var registration = ThreadPool.RegisterWaitForSingleObject(handle, (state, timedOut) =>
            {
                var localTcs = (TaskCompletionSource<object>)state;
                if (timedOut)
                    localTcs.TrySetCanceled();
                else
                    localTcs.TrySetResult(null);
            }, tcs, timeout, executeOnlyOnce: true);
            tcs.Task.ContinueWith((_, state) => 
                ((RegisteredWaitHandle)state).Unregister(null), registration, TaskScheduler.Default);
            return tcs.Task;
        }
    }
}