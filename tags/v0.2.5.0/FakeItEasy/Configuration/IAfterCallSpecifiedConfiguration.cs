using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.ComponentModel;

namespace FakeItEasy.Configuration
{
    /// <summary>
    /// Lets you set up expectations and configure repeat for the configured call.
    /// </summary>
    /// <typeparam name="TFake">The type of fake.</typeparam>
    public interface IAfterCallSpecifiedConfiguration<TFake>
            : IRepeatConfiguration<TFake>
    { }
}
