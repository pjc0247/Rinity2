using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RiniSharpCore.Impl
{
    public interface IPubSubMessage { }

    public class NotifyChangeMessage : IPubSubMessage
    {
        public string variableName { get; set; }
        public object newValue { get; set; }
    }
    public class NotifyRemoveMessage : IPubSubMessage
    {
        public string variableName { get; set; }
    }
}
