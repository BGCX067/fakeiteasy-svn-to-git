using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FakeItEasy.Configuration;

namespace FakeItEasy
{
    internal interface IEventRaiserArguments
    {
        object Sender { get; }
        EventArgs EventArguments { get; }
    }
}
