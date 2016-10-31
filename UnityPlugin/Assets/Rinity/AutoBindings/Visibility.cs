using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rinity.AutoBindings
{
    [Flags]
    public enum SubscriptionType 
    {
        SharedVariable,
        LocalVariable,
        PubSubMessage
    }

    public class Visibility : Attribute
    {
        public SubscriptionType type;

        public Visibility(SubscriptionType type)
        {
            this.type = type;
        }
    }
}
