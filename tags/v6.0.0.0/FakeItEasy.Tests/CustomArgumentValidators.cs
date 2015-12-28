using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using FakeItEasy.Api;
using System.Diagnostics;
using FakeItEasy.Extensibility;
using System.Linq.Expressions;
using FakeItEasy.Expressions;

namespace FakeItEasy.Tests
{
    public static class CustomArgumentValidators
    {
        [ArgumentValidator(typeof(EquivalentToValidator<>))]
        public static IEnumerable<T> SameSequence<T>(this IExtensibleIs extensionPoint, IEnumerable<T> collection)
        {
            throw new NotImplementedException();
        }

        [ArgumentValidator(typeof(EquivalentToValidator<>))]
        public static IEnumerable<T> SameSequence<T>(this IExtensibleIs extensionPoint, params T[] collection)
        {
            throw new NotImplementedException();
        }

        private class EquivalentToValidator<T>
            : IArgumentValidator
        {
            private IEnumerable<T> comparedCollection;

            public EquivalentToValidator(IEnumerable<T> comparedCollection)
            {
                this.comparedCollection = comparedCollection;
            }

            public bool IsValid(object argument)
            {
                var collection = argument as IEnumerable<T>;

                return collection != null && collection.SequenceEqual(this.comparedCollection);
            }

            public override string ToString()
            {
                var builder = new StringBuilder();

                foreach (var item in this.comparedCollection)
                {
                    if (builder.Length > 0)
                    {
                        builder.Append(", ");
                    }

                    builder.Append(item.ToString());
                }

                return "<SAME SEQUENCE AS ({0})>".FormatInvariant(builder.ToString());
            }
        }

        [ArgumentValidator(typeof(ExpressionThatProducesValueValidator))]
        public static Expression ValueExpression(this IExtensibleIs extensionPoint, object expectedValue)
        {
            throw new NotSupportedException();
        }

        private class ExpressionThatProducesValueValidator
            : IArgumentValidator
        {
            private object expectedValue;

            public ExpressionThatProducesValueValidator(object expectedValue)
            {
                this.expectedValue = expectedValue;
            }

            public bool IsValid(object argument)
            {
                var expression = argument as Expression;
                return expression != null && object.Equals(expectedValue, ExpressionManager.GetValueProducedByExpression(expression));
            }

            public override string ToString()
            {
                return "<Expression that produce the value: {0}>".FormatInvariant(this.expectedValue);
            }
        }
    }
}
