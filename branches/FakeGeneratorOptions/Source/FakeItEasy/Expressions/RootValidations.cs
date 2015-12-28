using System;
namespace FakeItEasy.Expressions
{
    internal class RootValidations<T>
        : ArgumentValidatorScope<T>
    {
        internal override bool IsValid(T argument)
        {
            return true;
        }

        internal override bool ResultOfChildValidatorIsValid(bool result)
        {
            return result;
        }
    }
}
