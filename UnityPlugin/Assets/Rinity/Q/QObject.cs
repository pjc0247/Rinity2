using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using UnityEngine;

namespace Rinity.Q
{
    public class QObject : MonoBehaviour
    {
        void Awake()
        {
            Q.SetMainThreadId(Thread.CurrentThread.ManagedThreadId);
        }

        void Update()
        {
            Q.Step();
        }
    }
}
