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
using System.Collections;

namespace FakeItEasy.Tests
{
    public static class CustomArgumentValidators
    {
        public static ArgumentValidator<T> IsThisSequence<T>(this ArgumentValidations<T> validations, T collection) where T : IEnumerable
        {
            return ArgumentValidator<T>.Create(x => x.Cast<object>().SequenceEqual(collection.Cast<object>()), "Same sequence");
        }

        public static ArgumentValidator<T> IsThisSequence<T>(this ArgumentValidations<T> validations, params object[] collection) where T : IEnumerable
        {
            return ArgumentValidator<T>.Create(x => x.Cast<object>().SequenceEqual(collection.Cast<object>()), "Same sequence");
        }

        
        public static ArgumentValidator<Expression> ProducesValue(this ArgumentValidations<Expression> validations, object expectedValue)
        {
            return ArgumentValidator<Expression>.Create(x => object.Equals(expectedValue, ExpressionManager.GetValueProducedByExpression(x)), "Expression that produces the value {0}".FormatInvariant(expectedValue));
        }
    }
}
