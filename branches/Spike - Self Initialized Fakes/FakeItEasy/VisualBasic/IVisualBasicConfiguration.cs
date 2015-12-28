using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FakeItEasy.Configuration;

namespace FakeItEasy.VisualBasic
{
    public interface IVisualBasicConfiguration<TFake>
            : IVoidConfiguration<TFake>
    {
        IVoidConfiguration<TFake> WhenArgumentsMatch(Func<ArgumentCollection, bool> argumentsPredicate);
    }
}
