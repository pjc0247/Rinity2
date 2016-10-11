using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading;

using UnityEngine;

namespace RiniSharpCore.Impl
{
    public delegate void RiniAction();

    public class Dispatcher
    {
        private static int mainThreadId { get; set; }
        private static Queue taskQueue { get; set; }

        static Dispatcher()
        {
            taskQueue = Queue.Synchronized(new Queue());
            mainThreadId = -1;
        }

        public static void Dispatch(int to, RiniAction task)
        {
            if (to == (int)ThreadType.ThreadPool)
                ThreadPool.QueueUserWorkItem(_ => task());
            else if (to == (int)ThreadType.MainThread)
                taskQueue.Enqueue(task);
            else
                throw new ArgumentException("invalid to");
        }
        public static void DispatchAffinity(int to, RiniAction task)
        {
            if (to == (int)ThreadType.ThreadPool)
            {
                if (Thread.CurrentThread.IsThreadPoolThread)
                {
                    task();
                    return;
                }
            }
            else if (to == (int)ThreadType.MainThread)
            {
                if (Thread.CurrentThread.ManagedThreadId == mainThreadId)
                {
                    task();
                    return;
                }
            }

            Dispatch(to, task);
        }

        public static void Flush()
        {
            while(taskQueue.Count != 0)
            {
                var task = (RiniAction)taskQueue.Dequeue();

                task();
            }
        }
    }
}
