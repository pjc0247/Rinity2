using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RiniSharpCore.Impl
{
    class Unity
    {
        public static void Enter(string msg)
        {
            UnityEngine.Profiler.BeginSample(msg);
        }
        public static void Leave()
        {
            UnityEngine.Profiler.EndSample();
        }
    }
}
