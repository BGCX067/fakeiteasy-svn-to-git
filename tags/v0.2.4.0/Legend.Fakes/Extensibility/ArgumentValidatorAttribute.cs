using System;

namespace Legend.Fakes.Extensibility
{
    /// <summary>
    /// Specifies that a custom argument validator should be used to validate
    /// arguments in an expression instead of using the value produced by the tagged
    /// method.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class ArgumentValidatorAttribute
        : Attribute
    {
        
        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="validatorType">The type of <see cref="IArgumentValidator" /> to use.</param>
        public ArgumentValidatorAttribute(Type validatorType)
        {
            this.ValidatorType = validatorType;
        }

        /// <summary>
        /// The type of <see cref="IArgumentValidator" /> to use.
        /// </summary>
        public Type ValidatorType
        {
            get;
            private set;
        }
    }
}