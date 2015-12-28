namespace FakeItEasy.Expressions
{
    using System.Linq.Expressions;
    using FakeItEasy.Extensibility;
    using System;

    public abstract class ArgumentValidator<T>
        : IArgumentValidator
    {
        private class PredicateArgumentValidator<T>
            : ArgumentValidator<T>
        {
            public Func<T, bool> Validation;
            public string DescriptionField;

            public override bool IsValid(T value)
            {
                return this.Validation.Invoke(value);
            }

            protected override string Description
            {
                get { return this.DescriptionField; }
            }
        }

        public T Argument
        {
            get
            {
                return default(T);
            }
        }

        protected abstract string Description { get; }

        public abstract bool IsValid(T value);

        bool IArgumentValidator.IsValid(object argument)
        {
            return this.IsValid((T)argument);
        }

        public override string ToString()
        {
            return string.Concat("<", this.Description, ">");
        }

        public static ArgumentValidator<T> Create(Func<T, bool> predicate, string description)
        {
            Guard.IsNotNull(predicate, "predicate");
            Guard.IsNotNullOrEmpty(description, "description");

            return new PredicateArgumentValidator<T>() { Validation = predicate, DescriptionField = description };
        }

        public static implicit operator T(ArgumentValidator<T> validator)
        {
            return default(T);
        }
    }
}
