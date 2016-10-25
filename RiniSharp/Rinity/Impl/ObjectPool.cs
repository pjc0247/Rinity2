using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rinity.Impl
{
    public class ObjectPool
    {
        public static object Get<T>()
            where T : new()
        {
            return new T();
        }

        public static void Return<T>(object obj)
        {
        }
    }
}
