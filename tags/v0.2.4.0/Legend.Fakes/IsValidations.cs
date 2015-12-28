using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using Legend.Fakes.Extensibility;
using System.Diagnostics.CodeAnalysis;

namespace Legend.Fakes
{
    public static class IsValidations
    {
        [ArgumentValidator(typeof(AnyArgumentValidator<>))]
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "extensionPoint")]
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        public static T Any<T>(this IExtensibleIs extensionPoint)
        {
            throw new NotSupportedException();
        }

        [ArgumentValidator(typeof(PredicateArgumentValidator<>))]
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "extensionPoint")]
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "predicate", Justification="It is used but only through reflection.")]
        public static T Match<T>(this IExtensibleIs extensionPoint, Func<T, bool> predicate)
        {
            throw new NotSupportedException();
        }

        [ArgumentValidator(typeof(NotNullValidator))]
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "extensionPoint")]
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        public static T NotNull<T>(this IExtensibleIs extensionPoint)
        {
            throw new NotSupportedException();
        }

        private class PredicateArgumentValidator<T>
            : IArgumentValidator
        {
            private Func<T, bool> predicate;

            public PredicateArgumentValidator(Func<T, bool> predicate)
            {
                this.predicate = predicate;
            }

            public bool IsValid(object argument)
            {
                return this.predicate.Invoke((T)argument);
            }
        }

        private class AnyArgumentValidator<T>
            : IArgumentValidator
        {
            public bool IsValid(object argument)
            {
                return true;
            }

            public override string ToString()
            {
                return "<Any {0}>".FormatInvariant(typeof(T).FullName);
            }
        }

        private class NotNullValidator
            : IArgumentValidator
        {
            public bool IsValid(object argument)
            {
                return argument != null;
            }

            public override string ToString()
            {
                return "<NOT NULL>";
            }
        }

    }

    
}
