using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Legend.Fakes.Configuration;

namespace Legend.Fakes.VisualBasic
{
    public interface IVisualBasicConfiguration<TFake>
            : IVoidConfiguration<TFake>
    {
        IVoidConfiguration<TFake> WhenArgumentsMatch(Func<ArgumentCollection, bool> argumentsPredicate);
    }
}
