using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rinity.Impl
{
    public interface IPubSubMessage
    {
        object sender { get; set; }
    }

    public class NotifyChangeMessage : IPubSubMessage
    {
        public object sender { get; set; }
        public string variableName { get; set; }
        public object newValue { get; set; }
    }
    public class NotifyRemoveMessage : IPubSubMessage
    {
        public object sender { get; set; }
        public string variableName { get; set; }
    }
}
