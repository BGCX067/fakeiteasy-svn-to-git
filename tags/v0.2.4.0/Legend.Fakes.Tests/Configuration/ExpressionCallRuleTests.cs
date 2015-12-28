using System;
using NUnit.Framework;
using System.Linq.Expressions;
using Legend.Fakes.Extensibility;
using System.Reflection;
using Legend.Fakes.Tests;
using Legend.Fakes.Api;

namespace Legend.Fakes.Configuration.Tests
{
    [TestFixture]
    public class ExpressionCallRuleTests 
    {
        [Test]
        public void Constructor_should_throw_when_expression_is_null()
        {
            Assert.Throws<ArgumentNullException>(
                () => CreateRule<IFoo>((Expression<Action<IFoo>>)null));
        }

        [Test]
        public void IsApplicableTo_should_throw_when_call_is_null()
        {
            var rule = CreateRule<IFoo>(x => x.Bar());

            Assert.Throws<ArgumentNullException>(
                () => rule.IsApplicableTo((IWritableFakeObjectCall)null));
        }

        [Test]
        public void IsApplicableTo_should_return_false_when_method_doesnt_match()
        {
            var rule = CreateRule<IFoo>(x => x.Bar());

            var nonMatchingCall = FakeCall.Create<IFoo>("Baz", new Type[] {}, new object[] {}); 
            
            Assert.That(rule.IsApplicableTo(nonMatchingCall), Is.False);
        }

        [Test]
        public void IsApplicableTo_should_return_true_when_method_matches_and_no_arguments_in_call()
        {
            var rule = CreateRule<IFoo>(x => x.Bar());

            var matchingCall = FakeCall.Create<IFoo>("Bar");
            
            Assert.That(rule.IsApplicableTo(matchingCall), Is.True);
        }

        [Test]
        public void IsApplicableTo_should_return_false_when_argument_doesnt_match()
        {
            var rule = CreateRule<IFoo>(x => x.Bar("test"));

            var nonMatchingCall = FakeCall.Create<IFoo>("Bar", new[] { typeof(object) }, new[] { "non matching" });

            Assert.That(rule.IsApplicableTo(nonMatchingCall), Is.False);
        }

        [Test]
        public void IsApplicableTo_should_return_true_when_argument_validator_is_passed_for_argument_and_it_validatees()
        {
            var rule = CreateRule<IFoo>(x => x.Bar(Argument.Is.Any<object>()));
            var call = CreateCallToBar("anything");
            
            Assert.That(rule.IsApplicableTo(call), Is.True);
        }

        [Test]
        public void IsApplicableTo_should_return_true_when_call_is_made_to_virtual_method_on_superclass()
        {
            var call = FakeCall.Create<SubClass>("CallMe");

            var rule = CreateRule<SubClass>(x => x.CallMe());

            Assert.That(rule.IsApplicableTo(call), Is.True);
        }

        [Test]
        public void IsApplicableTo_should_return_true_for_property_get_method_when_property_is_read_only()
        {
            var call = new FakeCall
            {
                FakedObject = A.Fake<IFoo>(),
                Arguments = ArgumentCollection.Empty,
                Method = typeof(IFoo).GetProperty("ReadOnlyProperty").GetGetMethod()
            };

            var rule = CreateRule<IFoo, string>(x => x.ReadOnlyProperty);

            Assert.That(rule.IsApplicableTo(call));
        }

        [Test]
        public void IsApplicableTo_should_return_false_for_random_method_when_property_is_read_only()
        {
            var call = FakeCall.Create<IFoo>("Bar", new Type[] { }, new object[] { });

            var rule = CreateRule<IFoo, string>(x => x.ReadOnlyProperty);

            Assert.That(rule.IsApplicableTo(call), Is.False);
        }
        
        [Test]
        public void Methods_that_are_not_tagged_with_the_ArgumentValidatorAttribute_should_be_invoked()
        {

            var rule = CreateRule<IFoo>(x => x.Bar(Argument.Is.NoValidator("foo")));
            var call = CreateCallToBar("foo");

            Assert.That(rule.IsApplicableTo(call), Is.True);
        }


        [Test]
        public void Constructor_should_throw_when_parameters_to_it_extension_does_not_match_constructor_parameters_for_validator()
        {
            Assert.Throws<ArgumentValidationException>(() =>
                CreateRule<IFormattable>(x => x.ToString(Argument.Is.ArgumentMismatchBetweenExtensionAndValidator<string>("", 0), Argument.Is.Any<IFormatProvider>())));
        }

        [Test]
        public void Constructor_should_throw_when_type_parameters_to_it_extension_does_not_match_type_parameters_to_generic_validator_type()
        {
            Assert.Throws<ArgumentValidationException>(() =>
                CreateRule<IFormattable>(x => x.ToString(Argument.Is.TypeArgumentMismatchBetweenExtensionAndValidator(), Argument.Is.Any<IFormatProvider>())));
        }

        [Test]
        public void Constructor_should_throw_when_type_specified_in_argument_validator_attribute_doesnt_implement_IArgumentValidator()
        {
            Assert.Throws<ArgumentValidationException>(() =>
                CreateRule<IFoo>(x => x.Bar(Argument.Is.NonArgumentValidatorTypeInAttribute<object>())));
        }

        [Test]
        public void Constructor_should_throw_when_exception_is_lambda_with_non_method_call_expression_as_body()
        {
            Assert.Throws<ArgumentException>(
                () => new ExpressionCallRule(Expression.Lambda(Expression.Constant(null))));
        }

        [Test]
        public void Constructor_should_work_with_expression_of_property()
        {
            var rule = new ExpressionCallRule((Expression<Func<IFoo, int>>)(x => x.SomeProperty));
            
        }

        [Test]
        public void Apply_should_call_the_applicator_with_the_incoming_call()
        {
            IWritableFakeObjectCall callPassedToApplicator = null;
            var callPassedToRule = FakeCall.Create<IFoo>("Bar");

            var rule = CreateRule<IFoo>(x => x.Bar());
            rule.Applicator = x => callPassedToApplicator = x;

            rule.Apply(callPassedToRule);

            Assert.That(callPassedToApplicator, Is.SameAs(callPassedToRule));
        }

        [Test]
        public void Apply_should_throw_when_applicator_is_null()
        {
            var rule = CreateRule<IFoo>(x => x.Bar());

            Assert.Throws<InvalidOperationException>(
                () => rule.Apply(new FakeCall()));
        }

        [Test]
        public void NumberOfTimesToCall_should_be_settable_and_gettable()
        {
            var rule = CreateRule<IFoo>(x => x.Bar());
            rule.NumberOfTimesToCall = 10;

            Assert.That(rule.NumberOfTimesToCall, Is.EqualTo(10));
        }

        [Test]
        public void ToString_should_return_string_with_name_of_the_method_when_no_arguments()
        {
            var rule = CreateRule<IFoo>(x => x.Bar());

            Assert.That(rule.ToString(), Is.EqualTo("Legend.Fakes.Tests.IFoo.Bar()"));
        }

        [Test]
        public void ToString_should_return_string_with_each_argument_as_string()
        {
            var rule = CreateRule<IFoo>(x => x.Bar(123, "abc"));

            Assert.That(rule.ToString(), Is.EqualTo("Legend.Fakes.Tests.IFoo.Bar([System.Object] 123, [System.Object] abc)"));

        }

        [Test]
        public void ToString_should_return_string_with_name_of_the_property()
        {
            var rule = CreateRule<IFoo, int>(x => x.SomeProperty);

            Assert.That(rule.ToString(), Is.EqualTo("Legend.Fakes.Tests.IFoo.SomeProperty"));
        }

        [Test]
        public void ToString_should_write_NULL_for_null_arguments()
        {
            var rule = CreateRule<IFoo>(x => x.Bar(null));

            Assert.That(rule.ToString(), Is.EqualTo("Legend.Fakes.Tests.IFoo.Bar([System.Object] <NULL>)"));
        }

        [Test]
        public void ToString_should_write_ANY_and_type_for_argument_validator_ANY()
        {
            var rule = CreateRule<IFoo>(x => x.Bar(Argument.Is.Any<string>()));

            Assert.That(rule.ToString(), Is.EqualTo("Legend.Fakes.Tests.IFoo.Bar([System.Object] <Any System.String>)"));
        }

        private static IWritableFakeObjectCall CreateCallToBar(object argument)
        {
            var call = FakeCall.Create<IFoo>("Bar", new[] { typeof(object) }, new[] { argument });
            
            return call;
        }

        private static ExpressionCallRule CreateRule<T>(Expression<Action<T>> expression)
        {
            return new ExpressionCallRule(expression);
        }

        private static ExpressionCallRule CreateRule<T, TReturn>(Expression<Func<T, TReturn>> expression)
        {
            return new ExpressionCallRule(expression);
        }


        public class SuperClass
        {
            public virtual void CallMe()
            {
                // do nothing
            }
        }

        public class SubClass
            : SuperClass
        {
            public override void CallMe()
            {
                base.CallMe();
                // do nothing
            }
        }
    }

    public static class FakeIsValidations
    {
        public static string NoValidator(this IExtensibleIs extensionPoint, string returnValue)
        {
            return returnValue;
        }


        [ArgumentValidator(typeof(NonMatchingParametersValidator))]
        public static object NonMatchingParameters(this IExtensibleIs i, string parameter, string parameter2)
        {
            throw new NotImplementedException();
        }

        private class NonMatchingParametersValidator
            : IArgumentValidator
        {
            public NonMatchingParametersValidator(string onlyOneParameter)
            {
                throw new NotImplementedException();
            }

            public bool IsValid(object argument)
            {
                throw new NotImplementedException();
            }
        }

        [ArgumentValidator(typeof(ValidatorThatTakesATypeArgument<>))]
        public static string TypeArgumentMismatchBetweenExtensionAndValidator(this IExtensibleIs extensionPoint)
        {
            throw new NotImplementedException();
        }

        private class ValidatorThatTakesATypeArgument<T>
            : IArgumentValidator
        {

            public bool IsValid(object argument)
            {
                throw new NotImplementedException();
            }
        }

        [ArgumentValidator(typeof(ValidatorThatTakesOneStringInTheConstructor))]
        public static T ArgumentMismatchBetweenExtensionAndValidator<T>(this IExtensibleIs extensionPoint, string anArgument, int anotherArgument)
        {
            throw new NotImplementedException();
        }


        public class ValidatorThatTakesOneStringInTheConstructor
            : IArgumentValidator
        {
            public ValidatorThatTakesOneStringInTheConstructor(string foo)
            {

            }

            public bool IsValid(object argument)
            {
                throw new NotImplementedException();
            }
        }

        [ArgumentValidator(typeof(string))]
        public static T NonArgumentValidatorTypeInAttribute<T>(this IExtensibleIs extensionPoint)
        {
            throw new NotSupportedException();
        }
    }

}