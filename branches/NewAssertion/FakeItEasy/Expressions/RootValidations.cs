namespace FakeItEasy.Expressions
{
    internal class RootValidations<T>
        : ArgumentValidations<T>
    {
        internal override bool IsValid(T argument)
        {
            return true;
        }
    }
}
