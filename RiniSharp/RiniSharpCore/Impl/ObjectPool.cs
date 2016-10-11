using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RiniSharpCore.Impl
{
    public class ObjectPool
    {
        public static object Get<T>()
            where T : new()
        {
            UnityEngine.Debug.Log("ObjectPool::Get");

            return new T();
        }

        public static void Return<T>(object obj)
        {
            UnityEngine.Debug.Log("ObjectPool::Return");
        }
    }
}
