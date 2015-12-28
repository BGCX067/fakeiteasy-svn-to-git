using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FakeItEasy.ExtensionSyntax;
using FakeItEasy.Extensibility;
using FakeItEasy.Expressions;
using System.Globalization;

namespace FakeItEasy.Examples
{
    public static class CustomArgumentValidators
    {
        public static ArgumentValidator<string> IsLongerThan(this ArgumentValidations<string> validations, int length)
        {
            return ArgumentValidator<string>.Create(x => x.Length > length, string.Format(CultureInfo.InvariantCulture, "Longer than {0}", length));
        }
    }
}
