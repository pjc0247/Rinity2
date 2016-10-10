using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RiniSharpCore
{
    public enum ThreadType
    {
        MainThread = 0,
        ThreadPool = 1
    }

    public class DispatchAttribute : Attribute
    {
        public ThreadType to;

        public DispatchAttribute(ThreadType to)
        {
            this.to = to;
        }
    }
}
