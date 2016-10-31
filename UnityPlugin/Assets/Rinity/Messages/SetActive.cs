using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Rinity.Impl;

namespace Rinity.Messages
{
    public class SetActive : IPubSubMessage
    {
        public object sender { get; set; }

        public bool active { get; set; }
    }
}
