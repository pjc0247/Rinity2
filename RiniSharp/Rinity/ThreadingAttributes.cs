using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rinity
{
    public enum ThreadType
    {
        MainThread = 0,
        ThreadPool = 1
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class DispatchAttribute : Attribute
    {
        public ThreadType type;

        public DispatchAttribute(ThreadType type)
        {
            this.type = type;
        }
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class AffinityAttribute : Attribute
    {
        public ThreadType type;

        public AffinityAttribute(ThreadType type)
        {
            this.type = type;
        }
    }
}
