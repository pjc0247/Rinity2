﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RiniSharp.Aspects
{
    class WeaveException : Exception
    {
        public WeaveException(string message)
            : base(message)
        {
        }
    }
}
