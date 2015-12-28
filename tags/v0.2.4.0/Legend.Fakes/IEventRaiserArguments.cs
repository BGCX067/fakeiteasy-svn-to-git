using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Legend.Fakes.Configuration;

namespace Legend.Fakes
{
    internal interface IEventRaiserArguments
    {
        object Sender { get; }
        EventArgs EventArguments { get; }
    }
}
