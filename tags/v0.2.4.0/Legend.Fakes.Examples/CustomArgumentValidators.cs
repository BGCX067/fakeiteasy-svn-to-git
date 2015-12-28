using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Legend.Fakes.ExtensionSyntax;
using Legend.Fakes.Extensibility;

namespace Legend.Fakes.Examples
{
    public static class CustomArgumentValidators
    {
        // Creating a custom argument validation involves some trickery but is not all that
        // complicated. First of all you need to create an extension method that extends the IExtensibleIs-type
        // and returns the type you want to validate (the method may be generic). This method should do
        // nothing but throw an exception. Then you have to create a class that has a constructor with
        // the same signature as the extension method but without the first (IExtensibleIs) argument,
        // this class should implement the IArgumentValidator-interface. Now add an ArgumentValidatorAttribute
        // to your extension method pointing to your created class and voila!
        [ArgumentValidator(typeof(IsLongerThanValidator))]
        public static string LongerThan(this IExtensibleIs extensionPoint, int length)
        {
            throw new NotSupportedException();
        }

        private class IsLongerThanValidator
            : IArgumentValidator
        {
            private int length;
            public IsLongerThanValidator(int length)
            {
                this.length = length;
            }

            public bool IsValid(object argument)
            {
                var s = argument as string;

                return s != null && s.Length > this.length;
            }
        }
    }
}
