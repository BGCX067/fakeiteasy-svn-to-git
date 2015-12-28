namespace FakeItEasy.Tests.Configuration
{
    using System;
    using System.Linq.Expressions;
    using FakeItEasy.Api;
    using FakeItEasy.Configuration;
    using FakeItEasy.Expressions;
    using FakeItEasy.Tests.TestHelpers;
    using NUnit.Framework;

    [TestFixture]
    public class StartConfigurationTests
    {
        private FakeObject fakeObject;
        private ExpressionCallRule rule;
        private ExpressionCallRule.Factory ruleFactory;
        private LambdaExpression argumentToRuleFactory;
        private IConfigurationFactory configurationFactory;

        [SetUp]
        public void SetUp()
        {
            this.OnSetUp();
        }

        protected virtual void OnSetUp()
        {
            this.fakeObject = new FakeObject();
            this.rule = ExpressionHelper.CreateRule<IFoo>(x => x.Bar());
            this.ruleFactory = x =>
                {
                    this.argumentToRuleFactory = x;
                    return this.rule;
                };

            this.configurationFactory = A.Fake<IConfigurationFactory>();
        }

        [TearDown]
        public void TearDown()
        {
            this.argumentToRuleFactory = null;
        }

        private StartConfiguration<T> CreateConfiguration<T>()
        {
            return new StartConfiguration<T>(this.fakeObject, this.ruleFactory, this.configurationFactory);
        }

        [Test]
        public void CallsTo_for_void_calls_should_return_configuration_from_configuration_factory()
        {
            // Arrange
            var returnedConfiguration = A.Fake<IVoidArgumentValidationConfiguration>();

            A.CallTo(() => this.configurationFactory.CreateConfiguration(this.fakeObject, this.rule)).Returns(returnedConfiguration);

            var configuration = this.CreateConfiguration<IFoo>();

            Expression<Action<IFoo>> callSpecification = x => x.Bar();

            // Act
            var result = configuration.CallsTo(callSpecification);

            // Assert
            Assert.That(argumentToRuleFactory, Is.SameAs(callSpecification));
            Assert.That(result, Is.SameAs(returnedConfiguration));
        }

        [Test]
        public void CallsTo_for_void_calls_should_add_rule_to_fake_object()
        {
            // Arrange
            var configuration = this.CreateConfiguration<IFoo>();

            // Act
            configuration.CallsTo(x => x.Bar());

            // Assert
            Assert.That(this.fakeObject.Rules, Has.Some.SameAs(this.rule));
        }

        [Test]
        public void CallsTo_for_void_calls_should_be_null_guarded()
        {
            // Arrange
            var configuration = this.CreateConfiguration<IFoo>();

            // Assert
            NullGuardedConstraint.Assert(() =>
                configuration.CallsTo(x => x.Bar()));
        }

        [Test]
        public void CallsTo_for_void_calls_should_set_applicator_to_do_nothing_as_default()
        {
            // Arrange
            var configuration = this.CreateConfiguration<IFoo>();

            // Act
            configuration.CallsTo(x => x.Bar());

            // Assert
            Assert.DoesNotThrow(() =>
                this.rule.Applicator(A.Fake<IWritableFakeObjectCall>()));
        }

        [Test]
        public void CallsTo_for_function_calls_should_return_configuration_from_configuration_factory()
        {
            // Arrange
            var returnedConfiguration = A.Fake<IReturnValueArgumentValidationConfiguration<int>>();

            A.CallTo(() => this.configurationFactory.CreateConfiguration<int>(this.fakeObject, this.rule)).Returns(returnedConfiguration);

            var configuration = this.CreateConfiguration<IFoo>();

            Expression<Func<IFoo, int>> callSpecification = x => x.Baz();

            // Act
            var result = configuration.CallsTo(callSpecification);

            // Assert
            Assert.That(argumentToRuleFactory, Is.SameAs(callSpecification));
            Assert.That(result, Is.SameAs(returnedConfiguration));
        }

        [Test]
        public void CallsTo_for_function_calls_should_add_rule_to_fake_object()
        {
            // Arrange
            var configuration = this.CreateConfiguration<IFoo>();

            // Act
            configuration.CallsTo(x => x.Baz());

            // Assert
            Assert.That(this.fakeObject.Rules, Has.Some.SameAs(this.rule));
        }

        [Test]
        public void CallsTo_for_function_calls_should_be_null_guarded()
        {
            // Arrange
            var configuration = this.CreateConfiguration<IFoo>();

            // Assert
            NullGuardedConstraint.Assert(() =>
                configuration.CallsTo(x => x.Baz()));
        }

        [Test]
        public void AnyCall_should_return_configuration_from_factory_with_AnyCallRule_set()
        {
            // Arrange
            var returnedConfiguration = A.Fake<IVoidArgumentValidationConfiguration>();

            Configure.Fake(this.configurationFactory)
                .CallsTo(x => x.CreateConfiguration(this.fakeObject, A<BuildableCallRule>.That.IsInstanceOf<AnyCallCallRule>()))
                .Returns(returnedConfiguration);

            var configuration = this.CreateConfiguration<IFoo>();

            // Act
            var result = configuration.AnyCall();

            // Assert
            Assert.That(result, Is.SameAs(returnedConfiguration));
        }

        [Test]
        public void AnyCall_should_add_rule_to_fake_object()
        {
            // Arrange
            var configuration = this.CreateConfiguration<int>();

            // Act
            configuration.AnyCall();

            // Assert
            Assert.That(this.fakeObject.Rules, Has.Some.InstanceOf<AnyCallCallRule>());
        }
    }
}
