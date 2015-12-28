using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FakeItEasy.Extensibility;

namespace FakeItEasy.Expressions.ArgumentValidators
{
    public class NotNullValidator
        : IArgumentValidator
    {
        public bool IsValid(object argument)
        {
            return argument != null;
        }

        public override string ToString()
        {
            return "<NOT NULL>";
        }
    }
}
