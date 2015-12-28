namespace FakeItEasy.Expressions
{
    using FakeItEasy.Extensibility;

    public class AnyValidator<T>
                : IArgumentValidator
    {
        public bool IsValid(object argument)
        {
            if (argument != null)
            {
                return argument is T;
            }

            return default(T) == null;
        }

        public override string ToString()
        {
            return "<Any {0}>".FormatInvariant(typeof(T).Name);
        }
    }
}