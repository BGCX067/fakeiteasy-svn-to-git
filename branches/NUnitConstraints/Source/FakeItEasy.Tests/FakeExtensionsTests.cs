using System;
using FakeItEasy.Configuration;
using NUnit.Framework;
using System.Linq;
using FakeItEasy.Api;
using System.Linq.Expressions;

namespace FakeItEasy.Tests
{
    [TestFixture]
    public class FakeExtensionsTests
    {
        [Test]
        public void Once_should_call_NumberOfTimes_with_1_as_argument()
        {
            var repeatConfig = A.Fake<IRepeatConfiguration>();

            repeatConfig.Once();

            A.CallTo(() => repeatConfig.NumberOfTimes(1)).Assert(Happened.Once);
        }

        [Test]
        public void Once_should_throw_when_configuration_is_null()
        {
            Assert.Throws<ArgumentNullException>(() =>
                FakeExtensions.Once((IRepeatConfiguration)null));
        }

        [Test]
        public void Twice_should_call_NumberOfTimes_with_2_as_argument()
        {
            var repeatConfig = A.Fake<IRepeatConfiguration>();
            
            repeatConfig.Twice();

            A.CallTo(() => repeatConfig.NumberOfTimes(2)).Assert(Happened.Once);
        }

        [Test]
        public void Twice_should_throw_when_configuration_is_null()
        {
            Assert.Throws<ArgumentNullException>(() =>
                FakeExtensions.Twice((IRepeatConfiguration)null));
        }

        [Test]
        public void ReturnsNull_should_set_call_returns_with_null_on_configuration()
        {
            var config = A.Fake<IReturnValueConfiguration<string>>();
            config.ReturnsNull();

            A.CallTo(() => config.Returns(A<string>.That.IsNull())).Assert(Happened.Once);
        }

        [Test]
        public void ReturnsNull_should_return_configuration_object()
        {
            var config = A.Fake<IReturnValueConfiguration<string>>();
            var returnConfig = A.Fake<IAfterCallSpecifiedWithOutAndRefParametersConfiguration>();
            
            Configure.Fake(config).CallsTo(x => x.Returns(A<string>.Ignored)).Returns(returnConfig);

            var returned = config.ReturnsNull();
            var f = new Fake<IFoo>();
            
            Assert.That(returned, Is.SameAs(returnConfig));
        }

        [Test]
        public void ReturnsNull_should_be_null_guarded()
        {
            NullGuardedConstraint.Assert(() =>
                FakeExtensions.ReturnsNull(A.Fake<IReturnValueConfiguration<string>>()));
        }

        [Test]
        public void WithAnyArguments_with_void_call_should_call_when_arguments_match_with_predicate_that_returns_true()
        {
            var configuration = A.Fake<IArgumentValidationConfiguration<IVoidConfiguration>>();

            FakeExtensions.WithAnyArguments(configuration);

            var predicate = Fake.GetCalls(configuration).Single().Arguments.Get<Func<ArgumentCollection, bool>>(0);

            Assert.That(predicate.Invoke(null), Is.True);
        }

        [Test]
        public void WithAnyArguments_with_function_call_should_call_when_arguments_match_with_predicate_that_returns_true()
        {
            var configuration = A.Fake<IArgumentValidationConfiguration<IReturnValueConfiguration<int>>>();

            FakeExtensions.WithAnyArguments(configuration);

            var predicate = Fake.GetCalls(configuration).Single().Arguments.Get<Func<ArgumentCollection, bool>>(0);

            Assert.That(predicate.Invoke(null), Is.True);
        }
    }
}
