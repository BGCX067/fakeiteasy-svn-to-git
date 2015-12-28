using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace Legend.Fakes.Extensibility
{
    public interface IArgumentValidator
    {
        bool IsValid(object argument);
    }
}
