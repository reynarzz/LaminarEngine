using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Layers
{
    internal class MainThreadDispatcher : LayerBase
    {
        private static readonly ConcurrentQueue<Func<Task>> _queue = new();
        public override void Close()
        {
        }

        internal override void UpdateLayer()
        {
            DispatchAll();
        }

        internal static void DispatchAll()
        {
            while (_queue.TryDequeue(out var action))
            {
                try
                {
                    action();
                }
                catch (Exception e)
                {
                    Debug.Error(e);
                }
            }
        }
        public static Task<T> EnqueueAsync<T>(Func<T> action)
        {
            var tcs = new TaskCompletionSource<T>();
            _queue.Enqueue(() =>
            {
                try
                {
                    var value = action();
                    tcs.SetResult(value);
                }
                catch (Exception ex)
                {
                    tcs.SetException(ex);
                }
                return Task.CompletedTask;
            });
            return tcs.Task;
        }
    }
}
