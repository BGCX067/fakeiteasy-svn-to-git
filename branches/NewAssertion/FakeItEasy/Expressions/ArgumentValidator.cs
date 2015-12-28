namespace FakeItEasy.Expressions
{
    using System.Linq.Expressions;
    using FakeItEasy.Extensibility;
    using System;

    public abstract class ArgumentValidator<T>
        : IArgumentValidator
    {
        protected ArgumentValidator(ArgumentValidations<T> validations)
        {
            this.Validations = validations;
        }

        private class PredicateArgumentValidator
            : ArgumentValidator<T>
        {
            public PredicateArgumentValidator(ArgumentValidations<T> validations)
                : base(validations)
            {
                
            }

            public Func<T, bool> Validation;
            public string DescriptionField;

            protected override string Description
            {
                get { return this.DescriptionField; }
            }

            protected override bool Evaluate(T value)
            {
                return this.Validation.Invoke(value);
            }
        }

        internal ArgumentValidations<T> Validations
        {
            get;
            private set;
        }

        public T Argument
        {
            get
            {
                return default(T);
            }
        }

        public ArgumentValidations<T> And
        {
            get
            {
                return new AndValidations() { ParentValidator = this };
            }
        }

        protected abstract string Description { get; }

        public bool IsValid(T value)
        {
            return this.Validations.IsValid(value) == this.Evaluate(value);
        }

        /// <summary>
        /// Gets the full description of the validator, together with any parent validations
        /// and validators descriptions.
        /// </summary>
        internal string FullDescription
        {
            get
            {
                var validationsDescription = this.Validations.ToString();

                if (string.IsNullOrEmpty(validationsDescription))
                {
                    return this.Description;
                }
                else
                {
                    return string.Concat(validationsDescription, " ", this.Description);
                }
            }
        }

        /// <summary>
        /// Allows you to combine the current validator with another validator, where only
        /// one of them has to be valid.
        /// </summary>
        /// <param name="otherValidator">The validator to combine with.</param>
        /// <returns>A combined validator.</returns>
        public ArgumentValidator<T> Or(ArgumentValidator<T> otherValidator)
        {
            return new OrValidator(this, otherValidator);
        }

        protected abstract bool Evaluate(T value);

        bool IArgumentValidator.IsValid(object argument)
        {
            return this.IsValid((T)argument);
        }

        public override string ToString()
        {
            return string.Concat("<", this.FullDescription, ">");
        }

        public static ArgumentValidator<T> Create(ArgumentValidations<T> validations, Func<T, bool> predicate, string description)
        {
            Guard.IsNotNull(validations, "validations");
            Guard.IsNotNull(predicate, "predicate");
            Guard.IsNotNullOrEmpty(description, "description");

            return new PredicateArgumentValidator(validations) { Validation = predicate, DescriptionField = description };
        }

        private class OrValidator
            : ArgumentValidator<T>
        {
            private ArgumentValidator<T> first;
            private ArgumentValidator<T> second;

            public OrValidator(ArgumentValidator<T> first, ArgumentValidator<T> second)
                : base(new RootValidations<T>())
            {
                this.first = first;
                this.second = second;
            }

            protected override string Description
            {
                get { return "{0} or ({1})".FormatInvariant(this.first.FullDescription, this.second.FullDescription); }
            }

            protected override bool Evaluate(T value)
            {
                return this.first.IsValid(value) || this.second.IsValid(value);
            }
        }

        private class AndValidations
            : ArgumentValidations<T>
        {
            public ArgumentValidator<T> ParentValidator;

            internal override bool IsValid(T argument)
            {
                return this.ParentValidator.IsValid(argument);
            }

            public override string ToString()
            {
                return string.Concat(this.ParentValidator.FullDescription, " and");
            }
        }

        public static implicit operator T(ArgumentValidator<T> validator)
        {
            return default(T);
        }
    }
}
