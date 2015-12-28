using System;
using FakeItEasy.Configuration;
using NUnit.Framework;
using System.Linq;
using System.Linq.Expressions;
using NUnit.Framework.Constraints;
using FakeItEasy.Api;
using FakeItEasy.Expressions;

namespace FakeItEasy.Tests.Configuration
{
    [TestFixture]
    public class FakeConfigurationTests
    {
        private FakeObject fake;
        private FakeConfiguration<IFoo> configuration;
        private ExpressionCallRule.Factory expressionRuleFactory;
        private ExpressionCallRule ruleProducedByFactory;

        private IFoo FakeFoo
        {
            get
            {
                return (IFoo)this.fake.Object;
            }
        }

        [SetUp]
        public void SetUp()
        {
            this.OnSetUp();
        }

        protected virtual void OnSetUp()
        {
            this.fake = new FakeObject(typeof(IFoo));
            this.expressionRuleFactory = x =>
                {
                    var result = new ExpressionCallRule(new ExpressionCallMatcher(x, A.Fake<ArgumentValidatorFactory>(), A.Fake<MethodInfoManager>()));
                    this.ruleProducedByFactory = result;
                    return result;
                };

            this.configuration = this.CreateConfiguration();
        }

        private FakeConfiguration<IFoo> CreateConfiguration()
        {
            return new FakeConfiguration<IFoo>(this.fake, this.expressionRuleFactory);
        }

        private FakeConfiguration<IFoo> CreateConfiguration(BuildableCallRule ruleBeingBuilt)
        {
            return new FakeConfiguration<IFoo>(this.fake, ruleBeingBuilt);
        }

        [Test]
        public void CallsTo_with_function_call_should_add_expression_rule()
        {
            var expression = CreateExpression<IFoo, int>(x => x.Baz());

            configuration.CallsTo(expression);

            Assert.That(this.fake.Rules.Any(x => x.Equals(this.ruleProducedByFactory)), Is.True);
        }

        [Test]
        public void CallsTo_with_void_call_should_add_expression_interceptor_to_fake_object()
        {
            var expression = CreateExpression<IFoo>(x => x.Bar());

            this.configuration.CallsTo(expression);

            Assert.That(this.fake.Rules.ToArray(), Has.Some.EqualTo(this.ruleProducedByFactory));
        }

        [Test]
        public void CallsTo_with_void_call_should_set_up_so_that_Apply_does_nothing()
        {
            var expression = CreateExpression<IFoo>(x => x.Bar());
            this.configuration.CallsTo(expression);

            var call = A.Fake<IWritableFakeObjectCall>();

            this.configuration.RuleBeingBuilt.Apply(call);
        }

        [Test]
        public void CallsTo_with_void_call_should_return_configuration_as_ICallConfiguration()
        {
            var result = this.configuration.CallsTo(x => x.Bar());
            
            Assert.That(result, Is.SameAs(this.configuration));
        }

        [Test]
        public void CallsTo_with_function_call_should_return_return_value_configuration()
        {
            var result = this.configuration.CallsTo(x => x.Baz()) as FakeConfiguration<IFoo>.ReturnValueConfiguration<int>;

            Assert.That(result.ParentConfiguration, Is.EqualTo(this.configuration));
        }

        [Test]
        public void CallsTo_with_void_call_should_set_created_interceptor_to_the_InterceptorBeingBuilt_property()
        {
            this.configuration.CallsTo(x => x.Bar());

            Assert.That(this.configuration.RuleBeingBuilt, Is.Not.Null);
        }

        [Test]
        public void CallsTo_with_function_call_should_set_created_interceptor_to_the_InterceptorBeingBuilt_property()
        {
            this.configuration.CallsTo(x => x.Baz());

            Assert.That(this.configuration.RuleBeingBuilt, Is.Not.Null);
        }

        [Test]
        public void Returns_called_with_value_sets_applicator_to_a_function_that_applies_that_value_to_interceptor()
        {
            var returnConfig = this.CreateTestableReturnConfiguration();
            var call = A.Fake<IWritableFakeObjectCall>();

            returnConfig.Returns(10);
            returnConfig.ParentConfiguration.RuleBeingBuilt.Apply(call);

            Fake.Assert(call).WasCalled(x => x.SetReturnValue(10));
        }

        [Test]
        public void Returns_called_with_value_returns_parent_configuration()
        {
            var returnConfig = this.CreateTestableReturnConfiguration();

            var result = returnConfig.Returns(10);

            Assert.That(result, Is.EqualTo(this.configuration));
        }

        [Test]
        public void Returns_called_with_delegate_sets_return_value_produced_by_delegate_each_time()
        {
            var returnConfig = this.CreateTestableReturnConfiguration();
            var call = A.Fake<IWritableFakeObjectCall>();
            int i = 1;
            
            returnConfig.Returns(() => i++);
            returnConfig.ParentConfiguration.RuleBeingBuilt.Apply(call);
            returnConfig.ParentConfiguration.RuleBeingBuilt.Apply(call);
            
            Fake.Assert(call).WasCalled(x => x.SetReturnValue(1));
            Fake.Assert(call).WasCalled(x => x.SetReturnValue(2));
        }

        [Test]
        public void Returns_with_delegate_throws_when_delegate_is_null()
        {
            var returnConfig = this.CreateTestableReturnConfiguration();
            
            Assert.Throws<ArgumentNullException>(() =>
                returnConfig.Returns((Func<int>)null));
        }

        [Test]
        public void Returns_called_with_delegate_returns_parent_cofiguration()
        {
            var returnConfig = this.CreateTestableReturnConfiguration();

            var result = returnConfig.Returns(() => 10);

            Assert.That(result, Is.EqualTo(this.configuration));
        }

        [Test]
        public void Returns_with_call_function_applies_value_returned_from_function()
        {
            var config = this.CreateTestableReturnConfiguration();

            var returnConfig = this.CreateTestableReturnConfiguration();
            var call = A.Fake<IWritableFakeObjectCall>();
            Fake.Configure(call).CallsTo(x => x.Arguments).Returns(new ArgumentCollection(
                new object[] { 1, 2 },
                new string[] { "foo", "bar" }));

            returnConfig.Returns(x => x.Arguments.Get<int>("bar"));

            config.ParentConfiguration.RuleBeingBuilt.Apply(call);

            Fake.Assert(call).WasCalled(x => x.SetReturnValue(2));
        }

        [Test]
        public void Returns_with_call_function_should_return_delegate()
        {
            var config = this.CreateTestableReturnConfiguration();

            var returned = config.Returns(x => x.Arguments.Get<int>(0));

            Assert.That(returned, Is.EqualTo(config.ParentConfiguration));
        }

        [Test]
        public void Returns_with_call_function_should_be_properly_guarded()
        {
            var config = this.CreateTestableReturnConfiguration();

            NullGuardedConstraint.Assert(() => config.Returns(x => x.Arguments.Get<int>(0)));
        }

        [Test]
        public void Throws_configures_interceptor_so_that_the_specified_exception_is_thrown_when_apply_is_called()
        {
            this.StubInterceptorBeingBuilt();

            var exception = new FormatException();

            this.configuration.Throws(exception);

            Assert.Throws<FormatException>(() =>
                this.configuration.RuleBeingBuilt.Apply(A.Fake<IWritableFakeObjectCall>()));
        }

        [Test]
        public void Throws_returns_configuration()
        {
            this.StubInterceptorBeingBuilt();

            var result = this.configuration.Throws(new Exception());

            Assert.That(result, Is.EqualTo(this.configuration));
        }

        [Test]
        public void Throws_called_from_return_value_configuration_configures_interceptor_so_that_the_specified_exception_is_thrown_when_apply_is_called()
        {
            var returnConfig = this.CreateTestableReturnConfiguration();
            
            var exception = new FormatException();

            returnConfig.Throws(exception);
            
            Assert.Throws<FormatException>(() =>
                this.configuration.RuleBeingBuilt.Apply(A.Fake<IWritableFakeObjectCall>()));
        }

        [Test]
        public void Throws_called_from_return_value_configuration_returns_parent_configuration()
        {
            var returnConfig = this.CreateTestableReturnConfiguration();

            var result = returnConfig.Throws(new Exception()) as FakeConfiguration<IFoo>;

            Assert.That(result, Is.EqualTo(this.configuration));
        }

        [Test]
        public void NumberOfTimes_sets_number_of_times_to_interceptor()
        {
            this.StubInterceptorBeingBuilt();

            this.configuration.NumberOfTimes(10);

            Assert.That(this.configuration.RuleBeingBuilt.NumberOfTimesToCall, Is.EqualTo(10));
        }

        [Test]
        public void NumberOfTimes_throws_when_number_of_times_is_not_a_positive_integer(
            [Values(0, -1, -100, int.MinValue)]int numberOftimes)
        {
            this.StubInterceptorBeingBuilt();

            Assert.Throws<ArgumentOutOfRangeException>(() => 
                this.configuration.NumberOfTimes(numberOftimes));
        }

        
        [Test]
        public void AnyCall_creates_call_rule_that_is_applicable_to_any_call()
        {
            this.configuration.AnyCall();

            Assert.That(this.configuration.RuleBeingBuilt.IsApplicableTo(A.Fake<IWritableFakeObjectCall>()), Is.True);
        }

        [Test]
        public void AnyCall_adds_rule_to_fake_object()
        {
            this.configuration.AnyCall();

            Assert.That(this.fake.Rules, Has.Some.EqualTo(this.configuration.RuleBeingBuilt));
        }

        [Test]
        public void AnyCall_returns_configuration_object()
        {
            var result = this.configuration.AnyCall();

            Assert.That(result, Is.EqualTo(this.configuration));
        }

        [Test]
        public void WhenArgumentsMatches_should_throw_when_rule_being_built_is_not_a_recorded_rule()
        {
            Assert.Throws<NotSupportedException>(() =>
                this.configuration.WhenArgumentsMatch(x => true));

        }

        [Test]
        public void DoesNothing_should_set_applicator_that_does_nothing_when_called()
        {
            this.StubInterceptorBeingBuilt();
            this.configuration.DoesNothing();

            var call = A.Fake<IWritableFakeObjectCall>();
            Fake.Configure(call).AnyCall().Throws(new AssertionException("Applicator should do nothing."));
            
            this.configuration.RuleBeingBuilt.Applicator(call);
        }

        [Test]
        public void Does_nothing_should_return_configuration_object()
        {
            this.StubInterceptorBeingBuilt();
            var result = this.configuration.DoesNothing();

            Assert.That(result, Is.EqualTo(this.configuration));
        }

        [Test]
        public void Invokes_should_return_the_configuration_object()
        {
            this.StubInterceptorBeingBuilt();
            var result = this.configuration.Invokes(x => { });

            Assert.That(result, Is.SameAs(this.configuration));
        }

        [Test]
        public void Invokes_should_add_action_to_list_of_actions()
        {
            this.StubInterceptorBeingBuilt();
            Action<IFakeObjectCall> action = x => { };

            this.configuration.Invokes(action);

            Assert.That(this.configuration.RuleBeingBuilt.Actions.ToArray(), 
                Has.Some.EqualTo(action));
        }

        [Test]
        public void Invokes_should_be_null_guarded()
        {
            Action<IFakeObjectCall> action = x => { };
            NullGuardedConstraint.Assert(() =>
                this.configuration.Invokes(action));
        }

        [Test]
        public void Invokes_on_return_value_configuration_should_return_the_configuration_object()
        {
            var returnConfig = this.CreateTestableReturnConfiguration();
            var result = returnConfig.Invokes(x => { });

            Assert.That(result, Is.SameAs(returnConfig));
        }

        [Test]
        public void Invokes_on_return_value_configuration_should_add_action_to_list_of_actions()
        {
            var returnConfig = this.CreateTestableReturnConfiguration();
            Action<IFakeObjectCall> action = x => { };

            returnConfig.Invokes(action);

            Assert.That(this.configuration.RuleBeingBuilt.Actions.ToArray(),
                Has.Some.EqualTo(action));
        }

        [Test]
        public void Invokes_on_return_value_configuration_should_be_null_guarded()
        {
            var returnConfig = this.CreateTestableReturnConfiguration();

            Action<IFakeObjectCall> action = x => { };
            NullGuardedConstraint.Assert(() =>
                returnConfig.Invokes(action));
        }

        [Test]
        public void CallsBaseMethod_sets_CallBaseMethod_to_true_on_the_built_rule()
        {
            this.StubInterceptorBeingBuilt();
            this.configuration.CallsBaseMethod();

            Assert.That(this.configuration.RuleBeingBuilt.CallBaseMethod, Is.True);
        }

        [Test]
        public void CallBaseMethod_returns_configuration_object()
        {
            this.StubInterceptorBeingBuilt();
            var result = this.configuration.CallsBaseMethod();

            Assert.That(result, Is.SameAs(this.configuration));
        }

        [Test]
        public void CallBaseMethod_sets_the_applicator_to_a_null_action()
        {
            this.StubInterceptorBeingBuilt();
            this.configuration.CallsBaseMethod();

            Assert.That(this.configuration.RuleBeingBuilt.Applicator, Is.Not.Null);
        }

        [Test]
        public void CallsBaseMethod_for_function_calls_sets_CallBaseMethod_to_true_on_the_built_rule()
        {
            var config = this.CreateTestableReturnConfiguration();
            config.CallsBaseMethod();

            Assert.That(this.configuration.RuleBeingBuilt.CallBaseMethod, Is.True);
        }

        [Test]
        public void CallBaseMethod_for_function_calls_returns_configuration_object()
        {
            var config = this.CreateTestableReturnConfiguration();
            var result = config.CallsBaseMethod();

            Assert.That(result, Is.SameAs(this.configuration));
        }

        [Test]
        public void CallBaseMethod_for_function_calls_sets_the_applicator_to_a_null_action()
        {
            var config = this.CreateTestableReturnConfiguration();
            config.CallsBaseMethod();

            Assert.That(this.configuration.RuleBeingBuilt.Applicator, Is.Not.Null);
        }

        [Test]
        public void AssertWasCalled_should_set_IsAssertion_to_true_of_recorded_rule()
        {
            var config = new FakeConfiguration<IFoo>(this.fake, new RecordedCallRule(A.Fake<MethodInfoManager>()));
            config.AssertWasCalled(x => true);

            Assert.That(((RecordedCallRule)config.RuleBeingBuilt).IsAssertion, Is.True);
        }

        [Test]
        public void AssertWasCalled_should_set_repeat_predicate_to_the_recorded_rule()
        {
            Func<int, bool> repeatPredicate = x => true;
            var config = new FakeConfiguration<IFoo>(this.fake, new RecordedCallRule(A.Fake<MethodInfoManager>()));
            config.AssertWasCalled(repeatPredicate);

            Assert.That(((RecordedCallRule)config.RuleBeingBuilt).RepeatPredicate, Is.SameAs(repeatPredicate));
        }

        [Test]
        public void AssertWasCalled_should_throw_when_built_rule_is_not_a_RecordedCallRule()
        {
            var ex = Assert.Throws<InvalidOperationException>(() =>
                this.configuration.AssertWasCalled(x => true));

            Assert.That(ex.Message, Is.EqualTo("Only RecordedCallRules can be used for assertions."));
        }

        [Test]
        public void AssertWasCalled_should_be_null_guarded()
        {
            NullGuardedConstraint.Assert(() =>
                this.configuration.AssertWasCalled(x => true));
        }

        private void StubInterceptorBeingBuilt()
        {
            this.configuration.CallsTo(x => x.Bar());
        }

        private FakeConfiguration<IFoo>.ReturnValueConfiguration<int> CreateTestableReturnConfiguration()
        {
            return this.configuration.CallsTo(x => x.Baz()) as FakeConfiguration<IFoo>.ReturnValueConfiguration<int>;    
        }

        private static Expression<Func<TFake, TMember>> CreateExpression<TFake, TMember>(Expression<Func<TFake, TMember>> expression) where TFake : class
        {
            return expression;
        }

        private static Expression<Action<TFake>> CreateExpression<TFake>(Expression<Action<TFake>> expression)
            where TFake : class
        {
            return expression;
        }
    }
}
