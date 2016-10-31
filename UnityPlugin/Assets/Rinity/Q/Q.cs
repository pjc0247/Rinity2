using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using UnityEngine;

namespace Rinity.Q
{
    public enum QTargetThread
    {
        Any,
        SameThread,
        ThreadPool,
        MainThread
    }
    public enum QTaskOption
    {
        None,
        LightTask,
        LongRunning
    }

    public class Q
    {
        private static SortedList<int, QTask> tasks { get; set; }

        private static int currentTick
        {
            get
            {
                return Environment.TickCount;
            }
        }

        private static int mainThreadId { get; set; }
        public static bool isMainThread
        {
            get
            {
                return Thread.CurrentThread.ManagedThreadId == mainThreadId;
            }
        }

        static Q()
        {
            tasks = new SortedList<int, QTask>();
        }

        public static void SetMainThreadId(int id)
        {
            mainThreadId = id;
        }

        public static QTask Defer(Action callback, int time)
        {
            var task = QTask.Create(callback, QTargetThread.MainThread);

            lock (tasks)
                tasks.Add(currentTick + time, task);

            return task;
        }
        public static QTask Defer(Action callback)
        {
            return Defer(callback, 0);
        }

        public static QTask Async(Action callback, int time)
        {
            var task = QTask.Create(callback, QTargetThread.ThreadPool);
            Defer(() => { task.Invoke(); }, time);
            return task;
        }
        public static QTask Async(Action callback)
        {
            return Async(callback, 0);
        }

        internal static void Step()
        {
            SortedList<int, QTask> copy = null;

            lock (tasks)
                copy = new SortedList<int, QTask>(tasks);

            foreach(var task in copy)
            {
                if (currentTick < task.Key)
                    break;

                task.Value.Invoke();
            }
        }
    }

    public class QAsync
    {
        public static void Enqueue(Action callback)
        {
            ThreadPool.QueueUserWorkItem((x) => { callback(); });
        }
        public static void StartNew(Action callback)
        {
            new Thread(() => { callback(); }).Start();
        }
    }
    
    class QTaskState
    {
        public static readonly int Pending = 1;
        public static readonly int Running = 2;
        public static readonly int Canceled = 3;
        public static readonly int Done = 4;
    }

    public class QTask
    {
        private QTask parent { get; set; }

        private object syncObject;

        private Action callback { get; set; }
        private QTargetThread type { get; set; }
        private QTaskOption option { get; set; }

        private List<QTask> chains { get; set; }

        private int state = 0;

        public static QTask Create(QTask parent,
            Action callback,
            QTargetThread type,
            QTaskOption option = QTaskOption.None)
        {
            var task = new QTask(parent, callback, type, option);
            return task;
        }
        public static QTask Create(Action callback,
            QTargetThread type,
            QTaskOption option = QTaskOption.None)
        {
            var task = new QTask(null, callback, type, option);
            return task;
        }

        private QTask(QTask parent, Action callback, QTargetThread type, QTaskOption option)
        {
            this.syncObject = new object();

            this.parent = parent;
            this.callback = callback;
            this.chains = new List<QTask>();
            this.type = type;
            this.option = option;
        }

        public QTask ContinueWith(Action callback)
        {
            var task = QTask.Create(this, callback, QTargetThread.SameThread);
            chains.Add(task);
            return task;  
        }

        public void Cancel()
        {
            if (Interlocked.CompareExchange(ref state, QTaskState.Canceled, QTaskState.Pending) != QTaskState.Pending)
                throw new InvalidOperationException("");
        }

        public bool Wait(int timeout)
        {
            if (type == QTargetThread.MainThread && Q.isMainThread)
                throw new InvalidOperationException("");
            if (state == QTaskState.Done || state == QTaskState.Canceled)
                return true;

            if (option == QTaskOption.LightTask)
            {
                var targetTime = Environment.TickCount + timeout;

                for (int i = 100; i < 100 * 100; i += 20)
                {
                    Thread.SpinWait(i);

                    if (i > 1000)
                        Thread.Sleep(i / 1000);
                    if (state == QTaskState.Done)
                        return true;
                    if (Environment.TickCount >= targetTime)
                        return false;
                }

                timeout -= Environment.TickCount - targetTime;
            }

            lock (syncObject)
            {
                // Double-Check
                if (state == QTaskState.Done || state == QTaskState.Canceled)
                    return true;

                return Monitor.Wait(syncObject, timeout);
            }
        }
        public void Wait()
        {
            Wait(int.MaxValue);
        }

        private void InvokeSync()
        {
            var originalState = Interlocked.CompareExchange(ref state, QTaskState.Running, QTaskState.Pending);
            if (originalState == QTaskState.Pending)
                ;
            else if (originalState == QTaskState.Canceled)
                return;
            else
                throw new InvalidOperationException(); 

            callback();

            if (Interlocked.CompareExchange(ref state, QTaskState.Done, QTaskState.Running) != QTaskState.Running)
                throw new InvalidOperationException();

            lock (syncObject)
                Monitor.PulseAll(syncObject);

            InvokeChains();
        }
        public void Invoke()
        {
            if (type == QTargetThread.SameThread)
                InvokeSync();

            else if (type == QTargetThread.Any)
            {
                if (option == QTaskOption.LongRunning)
                    QAsync.StartNew(InvokeSync);
                else if (type == QTargetThread.MainThread)
                    QAsync.Enqueue(InvokeSync);
                else
                    InvokeSync();
            }

            else if (type == QTargetThread.MainThread)
            {
                if (Q.isMainThread)
                    InvokeSync();
                else
                    Q.Defer(InvokeSync);
            }

            else if (type == QTargetThread.ThreadPool)
            {
                if (Thread.CurrentThread.IsThreadPoolThread)
                    InvokeSync();
                else
                    QAsync.StartNew(InvokeSync);
            }
        }
        private void InvokeChains()
        {
            foreach (var chain in chains)
            {
                chain.Invoke();
            }
        }
    }
}
