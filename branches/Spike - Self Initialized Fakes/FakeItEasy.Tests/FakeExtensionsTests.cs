using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
using System.Linq.Expressions;
using Castle.Core.Interceptor;
using FakeItEasy.Configuration;
using FakeItEasy.Api;
using System.Collections;

namespace FakeItEasy.Tests
{
    [TestFixture]
    public class FakeExtensionsTests
    {
        [Test]
        public void Once_should_call_NumberOfTimes_with_1_as_argument()
        {
            var repeatConfig = A.Fake<IRepeatConfiguration<IFoo>>();

            repeatConfig.Once();

            Fake.Assert(repeatConfig).WasCalled(x => x.NumberOfTimes(1));
        }

        [Test]
        public void Once_should_throw_when_configuration_is_null()
        {
            Assert.Throws<ArgumentNullException>(() =>
                FakeExtensions.Once((IRepeatConfiguration<IFoo>)null));
        }

        [Test]
        public void Twice_should_call_NumberOfTimes_with_2_as_argument()
        {
            var repeatConfig = A.Fake<IRepeatConfiguration<IFoo>>();
            
            repeatConfig.Twice();

            Fake.Assert(repeatConfig).WasCalled(x => x.NumberOfTimes(2));
        }

        [Test]
        public void Twice_should_throw_when_configuration_is_null()
        {
            Assert.Throws<ArgumentNullException>(() =>
                FakeExtensions.Twice((IRepeatConfiguration<IFoo>)null));
        }
    }
}
