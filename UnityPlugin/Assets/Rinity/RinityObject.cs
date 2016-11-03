using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

using UnityEngine;

using Rinity;
using Rinity.Impl;

namespace Rinity
{
    public class RinityObject : MonoBehaviour
    {
        void Awake()
        {
            var tag = typeof(RinityObject).Assembly
                .GetTypes()
                .Where(x => x.Name == "AssemblyTag")
                .FirstOrDefault();

            if (tag == null)
                Debug.LogError("Failed to find AsseblTag.");
        }

        void Update()
        {
            Dispatcher.Flush();
        }
    }
}
