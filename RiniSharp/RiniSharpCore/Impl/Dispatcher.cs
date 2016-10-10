﻿using System;
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
        private static Queue taskQueue { get; set; }

        static Dispatcher()
        {
            taskQueue = Queue.Synchronized(new Queue());
        }

        public static void Dispatch(int to, RiniAction task)
        {
            UnityEngine.Debug.Log("DISPATCH " + to.ToString());

            if (to == (int)DispatchTo.ThreadPool)
                ThreadPool.QueueUserWorkItem(_ => task());
            else if (to == (int)DispatchTo.MainThread)
            {
                UnityEngine.Debug.Log("TOMAINTHREAD");
                taskQueue.Enqueue(task);
            }
            else
                throw new ArgumentException("invalid to");
        }

        public static void Flush()
        {
            UnityEngine.Debug.Log("FLUSH");

            while(taskQueue.Count != 0)
            {
                var task = (RiniAction)taskQueue.Dequeue();

                task();
            }
        }
    }
}
