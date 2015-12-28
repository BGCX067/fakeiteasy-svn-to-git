namespace FakeItEasy.Tests.Expressions
{
    using System;
    using System.Linq.Expressions;
    using FakeItEasy.Expressions;
    using FakeItEasy.Extensibility;
    using FakeItEasy.Tests.TestHelpers;
    using NUnit.Framework;

    [TestFixture]
    public class ExpressionCallMatcherTests
    {
        private ArgumentValidatorFactory validatorFactory;

        [SetUp]
        public void SetUp()
        {
            this.validatorFactory = A.Fake<ArgumentValidatorFactory>();
            var validator = A.Fake<IArgumentValidator>();
            Fake.Configure(validator).CallsTo(x => x.IsValid(Argument.Is.Any<object>())).Returns(true);
            Fake.Configure(validatorFactory).CallsTo(x => x.GetArgumentValidator(Argument.Is.Any<Expression>())).Returns(validator);
        }

        private ExpressionCallMatcher CreateMatcher<TFake, TMember>(Expression<Func<TFake, TMember>> callSpecification)
        {
            return CreateMatcher((LambdaExpression)callSpecification);
        }

        private ExpressionCallMatcher CreateMatcher<TFake>(Expression<Action<TFake>> callSpecification)
        {
            return CreateMatcher((LambdaExpression)callSpecification);
        }

        private ExpressionCallMatcher CreateMatcher(LambdaExpression callSpecification)
        {
            return new ExpressionCallMatcher(callSpecification, this.validatorFactory);
        }

        [Test]
        public void Constructor_should_throw_if_expression_is_not_property_or_method()
        {
            Assert.Throws<ArgumentException>(() =>
                this.CreateMatcher<Foo, IServiceProvider>(x => x.ServiceProvider));
        }

        [Test]
        public void Matches_returns_false_when_call_is_not_to_same_method()
        {
            var call = ExpressionHelper.CreateFakeCall<IFoo>(x => x.Bar());
            var matcher = this.CreateMatcher<IFoo, int>(x => x.Baz());
            
            Assert.That(matcher.Matches(call), Is.False);
        }

        [Test]
        public void Matches_returns_true_when_call_is_to_same_method()
        {
            var call = ExpressionHelper.CreateFakeCall<IFoo>(x => x.Bar());
            var matcher = this.CreateMatcher<IFoo>(x => x.Bar());

            Assert.That(matcher.Matches(call), Is.True);
        }

        [Test]
        public void Matches_returns_true_when_call_is_to_derived_method()
        {
            var call = ExpressionHelper.CreateFakeCall<SubType>(x => x.DoSomething());
            var matcher = this.CreateMatcher<BaseType>(x => x.DoSomething());

            Assert.That(matcher.Matches(call), Is.True);
        }

        [Test]
        public void Matches_returns_true_when_call_is_to_base_method()
        {
            var call = ExpressionHelper.CreateFakeCall<BaseType>(x => x.DoSomething());
            var matcher = this.CreateMatcher<SubType>(x => x.DoSomething());

            Assert.That(matcher.Matches(call), Is.True);
        }

        [Test]
        public void Matches_returns_false_when_call_is_to_same_method_but_with_different_generic_arguments()
        {
            var call = ExpressionHelper.CreateFakeCall<BaseType>(x => x.GetDefault<int>());
            var matcher = this.CreateMatcher<BaseType>(x => x.GetDefault<string>());

            Assert.That(matcher.Matches(call), Is.False);
        }

        [Test]
        public void Matches_returns_true_when_call_is_to_method_that_implements_matcher_interface_method()
        {
            var call = ExpressionHelper.CreateFakeCall<BaseType>(x => x.Equals(null));
            var matcher = this.CreateMatcher<IEquatable<BaseType>>(x => x.Equals((BaseType)null));

            Assert.That(matcher.Matches(call), Is.True);
        }

        [Test]
        public void Matches_returns_true_when_call_is_to_interface_method_that_is_implemented_by_matcher_method()
        {
            var call = ExpressionHelper.CreateFakeCall<IEquatable<BaseType>>(x => x.Equals(null));
            var matcher = this.CreateMatcher<BaseType>(x => x.Equals((BaseType)null));

            Assert.That(matcher.Matches(call), Is.True);
        }

        [Test]
        public void Matches_should_return_true_when_call_is_to_property_getter_of_expression_property()
        {
            var call = ExpressionHelper.CreateFakeCall<IFoo, int>(x => x.SomeProperty);
            var matcher = this.CreateMatcher<IFoo, int>(x => x.SomeProperty);

            Assert.That(matcher.Matches(call), Is.True);
        }

        [Test]
        public void Matches_should_use_ArgumentValidatorManager_to_create_validator_for_each_argument()
        {
            this.CreateMatcher<IFoo>(x => x.Bar("foo", 10));

            Fake.Assert(this.validatorFactory)
                .WasCalled(x => x.GetArgumentValidator(Argument.Is.ValueExpression("foo")));
            Fake.Assert(this.validatorFactory)
                .WasCalled(x => x.GetArgumentValidator(Argument.Is.ValueExpression(10)));
        }

        [Test]
        public void Matches_should_use_argument_validators_to_validate_each_argument_of_call()
        {
            var argument1 = "foo";
            var argument2 = 10;

            var validator = A.Fake<IArgumentValidator>();
            Fake.Configure(validator)
                .CallsTo(x => x.IsValid(Argument.Is.Any<object>()))
                .Returns(true);
            Fake.Configure(this.validatorFactory)
                .CallsTo(x => x.GetArgumentValidator(Argument.Is.Any<Expression>()))
                .Returns(validator);
            

            var call = ExpressionHelper.CreateFakeCall<IFoo>(x => x.Bar(argument1, argument2));
            var matcher = this.CreateMatcher<IFoo>(x => x.Bar(argument1, argument2));

            matcher.Matches(call);

            Fake.Assert(validator)
                .WasCalled(x => x.IsValid(argument1));
            Fake.Assert(validator)
                .WasCalled(x => x.IsValid(argument2));
        }

        [Test]
        public void Matches_should_return_false_when_any_argument_validator_returns_false()
        {
            var validator = A.Fake<IArgumentValidator>();
            Fake.Configure(validator).CallsTo(x => x.IsValid(Argument.Is.Any<object>())).Returns(false);
            Fake.Configure(validator).CallsTo(x => x.IsValid(Argument.Is.Any<object>())).Returns(true).Once();

            Fake.Configure(this.validatorFactory).CallsTo(x => x.GetArgumentValidator(Argument.Is.Any<Expression>())).Returns(validator);

            var call = ExpressionHelper.CreateFakeCall<IFoo>(x => x.Bar(1, 2));
            var matcher = this.CreateMatcher<IFoo>(x => x.Bar(1, 3));

            Assert.That(matcher.Matches(call), Is.False);
        }

        [Test]
        public void Matches_should_return_false_when_the_number_of_arguments_of_calls_does_not_match()
        {
            var call = ExpressionHelper.CreateFakeCall<IFoo>(x => x.Bar(1));
            var matcher = this.CreateMatcher<IFoo>(x => x.Bar(1, 2));

            Assert.That(matcher.Matches(call), Is.False);
        }

        [Test]
        public void Constructor_should_be_null_guarded()
        {
            Expression<Action<IFoo>> expression = x => x.Bar();
            NullGuardedConstraint.Assert(() =>
                new ExpressionCallMatcher(expression, this.validatorFactory));
        }

        [Test]
        public void ToString_should_write_full_method_name_with_type_name_and_arguments_list()
        {
            var validator = A.Fake<IArgumentValidator>();
            Fake.Configure(validator)
                .CallsTo(x => x.ToString())
                .Returns("<FOO>");

            Fake.Configure(this.validatorFactory)
                .CallsTo(x => x.GetArgumentValidator(Argument.Is.Any<Expression>()))
                .Returns(validator);

            var matcher = this.CreateMatcher<IFoo>(x => x.Bar(1, 2));

            Assert.That(matcher.ToString(), Is.EqualTo("FakeItEasy.Tests.IFoo.Bar(<FOO>, <FOO>)"));
            Fake.Assert(this.validatorFactory)
                .WasCalled(x => x.GetArgumentValidator(Argument.Is.Any<Expression>()), repeat => repeat == 2);
        }

        public class BaseType
            : IEquatable<BaseType>
        {
            public virtual void DoSomething()
            { 
            
            }

            public void DoSomething(params object[] arguments)
            { 
            
            }

            public T GetDefault<T>()
            {
                return default(T);
            }

            public bool Equals(BaseType other)
            {
                return true;
            }
        }

        public class SubType
            : BaseType
        {
            public override void DoSomething()
            {
                
            }
        }
    }
}