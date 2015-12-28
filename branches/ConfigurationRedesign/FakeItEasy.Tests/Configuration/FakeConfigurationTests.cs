using System;
using FakeItEasy.Configuration;
using NUnit.Framework;
using System.Linq;
using System.Linq.Expressions;
using NUnit.Framework.Constraints;
using FakeItEasy.Api;
using FakeItEasy.Expressions;
using FakeItEasy.Tests.TestHelpers;
using System.Collections.Generic;

namespace FakeItEasy.Tests.Configuration
{
    [TestFixture]
    public class FakeConfigurationTests
    {
        private FakeObject fake;
        private FakeConfiguration configuration;
        private BuildableCallRule ruleProducedByFactory;

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
            this.fake = new FakeObject();
            this.ruleProducedByFactory = A.Fake<BuildableCallRule>();
            this.configuration = this.CreateConfiguration();
        }

        private FakeConfiguration CreateConfiguration()
        {
            return new FakeConfiguration(this.fake, this.ruleProducedByFactory);
        }

        private FakeConfiguration CreateConfiguration(BuildableCallRule ruleBeingBuilt)
        {
            return new FakeConfiguration(this.fake, ruleBeingBuilt);
        }

     
        [Test]
        public void Returns_called_with_value_sets_applicator_to_a_function_that_applies_that_value_to_interceptor()
        {
            var returnConfig = this.CreateTestableReturnConfiguration();
            var call = A.Fake<IWritableFakeObjectCall>();

            returnConfig.Returns(10);

            this.ruleProducedByFactory.Applicator(call);

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

            this.ruleProducedByFactory.Applicator(call);
            this.ruleProducedByFactory.Applicator(call);

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
            Configure.Fake(call).CallsTo(x => x.Arguments).Returns(new ArgumentCollection(
                new object[] { 1, 2 },
                new string[] { "foo", "bar" }));

            returnConfig.Returns(x => x.Arguments.Get<int>("bar"));

            this.ruleProducedByFactory.Applicator(call);

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
            var exception = new FormatException();

            this.configuration.Throws(exception);

            Assert.Throws<FormatException>(() =>
                this.ruleProducedByFactory.Applicator(A.Fake<IWritableFakeObjectCall>()));
        }

        [Test]
        public void Throws_returns_configuration()
        {
            var result = this.configuration.Throws(new Exception());

            Assert.That(result, Is.EqualTo(this.configuration));
        }

        [Test]
        public void Throws_called_from_return_value_configuration_configures_interceptor_so_that_the_specified_exception_is_thrown_when_apply_is_called()
        {
            var returnConfig = this.CreateTestableReturnConfiguration();
            
            var exception = new FormatException();

            returnConfig.Throws(exception);
            
            var thrown = Assert.Throws<FormatException>(() =>
                this.ruleProducedByFactory.Applicator(A.Fake<IWritableFakeObjectCall>()));
            Assert.That(thrown, Is.SameAs(exception));
        }

        [Test]
        public void Throws_called_from_return_value_configuration_returns_parent_configuration()
        {
            var returnConfig = this.CreateTestableReturnConfiguration();

            var result = returnConfig.Throws(new Exception()) as FakeConfiguration;

            Assert.That(result, Is.EqualTo(this.configuration));
        }

        [Test]
        public void NumberOfTimes_sets_number_of_times_to_interceptor()
        {
            this.configuration.NumberOfTimes(10);

            Assert.That(this.configuration.RuleBeingBuilt.NumberOfTimesToCall, Is.EqualTo(10));
        }

        [Test]
        public void NumberOfTimes_throws_when_number_of_times_is_not_a_positive_integer(
            [Values(0, -1, -100, int.MinValue)]int numberOftimes)
        {
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
        public void DoesNothing_should_set_applicator_that_does_nothing_when_called()
        {
            this.configuration.DoesNothing();

            var call = A.Fake<IWritableFakeObjectCall>();
            Configure.Fake(call).AnyCall().Throws(new AssertionException("Applicator should do nothing."));
            
            this.configuration.RuleBeingBuilt.Applicator(call);
        }

        [Test]
        public void Does_nothing_should_return_configuration_object()
        {
            var result = this.configuration.DoesNothing();

            Assert.That(result, Is.EqualTo(this.configuration));
        }

        [Test]
        public void Invokes_should_return_the_configuration_object()
        {
            var result = this.configuration.Invokes(x => { });

            Assert.That(result, Is.SameAs(this.configuration));
        }

        [Test]
        public void Invokes_should_add_action_to_list_of_actions()
        {
            Action<IFakeObjectCall> action = x => { };

            this.configuration.Invokes(action);

            Fake.Assert(this.configuration.RuleBeingBuilt.Actions)
                .WasCalled(x => x.Add(action));
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

            Fake.Assert(this.configuration.RuleBeingBuilt.Actions)
                .WasCalled(x => x.Add(action));
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
            this.configuration.CallsBaseMethod();

            Assert.That(this.configuration.RuleBeingBuilt.CallBaseMethod, Is.True);
        }

        [Test]
        public void CallBaseMethod_returns_configuration_object()
        {
            var result = this.configuration.CallsBaseMethod();

            Assert.That(result, Is.SameAs(this.configuration));
        }

        [Test]
        public void CallBaseMethod_sets_the_applicator_to_a_null_action()
        {
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
            var config = new FakeConfiguration(this.fake, new RecordedCallRule(A.Fake<MethodInfoManager>()));
            config.AssertWasCalled(x => true);

            Assert.That(((RecordedCallRule)config.RuleBeingBuilt).IsAssertion, Is.True);
        }

        [Test]
        public void AssertWasCalled_should_set_repeat_predicate_to_the_recorded_rule()
        {
            Func<int, bool> repeatPredicate = x => true;
            var config = new FakeConfiguration(this.fake, new RecordedCallRule(A.Fake<MethodInfoManager>()));
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

        [Test]
        public void WhenArgumentsMatches_should_call_UsePredicateToValidateArguments_on_built_rule()
        {
            Func<ArgumentCollection, bool> predicate = x => true;

            var builtRule = A.Fake<BuildableCallRule>();

            var config = new FakeConfiguration(this.fake, builtRule);
            config.WhenArgumentsMatch(predicate);

            Fake.Assert(builtRule)
                .WasCalled(x => x.UsePredicateToValidateArguments(predicate));
        }

        [Test]
        public void WhenArgumentsMatches_should_return_self()
        {
            var builtRule = A.Fake<BuildableCallRule>();

            var config = new FakeConfiguration(this.fake, builtRule);
            
            Assert.That(config.WhenArgumentsMatch(x => true), Is.SameAs(config));
        }

        [Test]
        public void WhenArgumentsMatches_should_be_null_guarded()
        {
            var builtRule = A.Fake<BuildableCallRule>();

            var config = new FakeConfiguration(this.fake, builtRule);

            NullGuardedConstraint.Assert(() =>
                config.WhenArgumentsMatch(x => true));
        }

        [Test]
        public void WhenArgumentsMatches_with_function_call_should_call_UsePredicateToValidateArguments_on_built_rule()
        {
            var builtRule = A.Fake<BuildableCallRule>();
            var config = new FakeConfiguration(this.fake, builtRule);

            var returnConfig = new FakeConfiguration.ReturnValueConfiguration<bool>() { ParentConfiguration = config };
            
            Func<ArgumentCollection, bool> predicate = x => true;
            
            returnConfig.WhenArgumentsMatch(predicate);
           
            Fake.Assert(builtRule)
                .WasCalled(x => x.UsePredicateToValidateArguments(predicate));
        }

        [Test]
        public void WhenArgumentsMatches_with_function_call_should_return_config_should_return_self()
        {
            var returnConfig = this.CreateTestableReturnConfiguration();

            Assert.That(returnConfig.WhenArgumentsMatch(x => true), Is.SameAs(returnConfig));
        }

        [Test]
        public void WhenArgumentsMatches_with_function_call_should_be_null_guarded()
        {
            var returnConfig = this.CreateTestableReturnConfiguration();

            NullGuardedConstraint.Assert(() =>
                returnConfig.WhenArgumentsMatch(x => true));
        }

        //
        [Test]
        public void WhenArgumentsMatches_from_VB_should_be_null_guarded()
        {
            var builtRule = A.Fake<BuildableCallRule>();

            var config = new FakeConfiguration(this.fake, builtRule) as FakeItEasy.VisualBasic.IVisualBasicConfigurationWithArgumentValidation;

            NullGuardedConstraint.Assert(() =>
                config.WhenArgumentsMatch(x => true));
        }

        [Test]
        public void WhenArgumentsMatches_from_VB_should_return_configuration_object()
        {
            var builtRule = A.Fake<BuildableCallRule>();

            var config = new FakeConfiguration(this.fake, builtRule) as FakeItEasy.VisualBasic.IVisualBasicConfigurationWithArgumentValidation;

            var returned = config.WhenArgumentsMatch(x => true);

            Assert.That(returned, Is.SameAs(config));
        }

        [Test]
        public void WhenArgumentsMatches_from_VB_should_set_predicate_to_built_rule()
        {
            var builtRule = A.Fake<BuildableCallRule>();

            var config = new FakeConfiguration(this.fake, builtRule) as FakeItEasy.VisualBasic.IVisualBasicConfigurationWithArgumentValidation;

            Func<ArgumentCollection, bool> predicate = x => true;

            config.WhenArgumentsMatch(predicate);

            Fake.Assert(builtRule)
                .WasCalled(x => x.UsePredicateToValidateArguments(predicate));
        }

        [Test]
        public void AssignsOutAndRefParameters_should_be_null_guarded()
        {
            NullGuardedConstraint.Assert(() =>
                this.configuration.AssignsOutAndRefParameters());
        }

        [Test]
        public void AssignsOutAndRefParameters_should_set_values_to_rule()
        {
            this.configuration.AssignsOutAndRefParameters(1, "foo");

            Assert.That(this.ruleProducedByFactory.OutAndRefParametersValues, Is.EquivalentTo(new object[] { 1, "foo" }));
        }

        [Test]
        public void AssignsOutAndRefParameters_returns_self()
        {
            var result =this.configuration.AssignsOutAndRefParameters(1, "foo");

            Assert.That(result, Is.SameAs(this.configuration));
        }

        private FakeConfiguration.ReturnValueConfiguration<int> CreateTestableReturnConfiguration()
        {
            return new FakeConfiguration.ReturnValueConfiguration<int>() { ParentConfiguration = this.configuration };
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
