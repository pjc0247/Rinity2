﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

using Rinity;
using Rinity.Impl;

namespace Rinity
{
    public class Rinity : MonoBehaviour
    {
        void Awake()
        {

        }

        void Update()
        {
            Dispatcher.Flush();
        }
    }
}
