namespace FakeItEasy.Expressions
{
    using FakeItEasy.Extensibility;
    using System;

    public class PredicateValidator<T>
                : IArgumentValidator
    {
        private Func<T, bool> predicate;

        public PredicateValidator(Func<T, bool> predicate)
        {
            this.predicate = predicate;
        }

        public bool IsValid(object argument)
        {
            return this.predicate.Invoke((T)argument);
        }

        public override string ToString()
        {
            return "<predicate {0}>".FormatInvariant(typeof(T).FullName);
        }
    }
}
