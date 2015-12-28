namespace FakeItEasy.Expressions
{
    using FakeItEasy.Api;

    public class EqualityArgumentValidator
        : IArgumentValidator
    {
        private object expectedValue;

        public EqualityArgumentValidator(object expectedValue)
        {
            this.expectedValue = expectedValue;
        }

        public bool IsValid(object argument)
        {
            return object.Equals(this.expectedValue, argument);
        }

        public override string ToString()
        {
            if (this.expectedValue == null)
            {
                return "<NULL>";
            }

            var stringValue = this.expectedValue as string;
            if (stringValue != null)
            {
                return "\"{0}\"".FormatInvariant(stringValue);
            }

            return this.expectedValue.ToString();
        }
    }
}