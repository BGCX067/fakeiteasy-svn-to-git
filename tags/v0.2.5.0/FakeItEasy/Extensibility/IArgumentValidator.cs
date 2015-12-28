using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace FakeItEasy.Extensibility
{
    public interface IArgumentValidator
    {
        bool IsValid(object argument);
    }
}
