using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using FakeItEasy.Tests;

namespace FakeItEasy.IntegrationTests.Assertions
{
    [TestFixture]
    public class ExceptionMessagesTests
    {
        [Test]
        [SetCulture("en-US")]
        public void Exception_message_should_be_correctly_formatted()
        {
            var foo = A.Fake<IFoo>();

            foo.Bar();
            foo.Bar();

            foo.Bar("test");
            foo.Bar(new DateTime(1977, 4, 5), "birthday");
            foo.ToString();
            foo.Biz();
            
            var exception = Assert.Throws<ExpectationException>(() =>
                Fake.Assert(foo).WasCalled(x => x.Bar(""), repeat => repeat > 2));

            Assert.That(exception.Message, Is.EqualTo(@"
  Assertion failed for the following call:
    'FakeItEasy.Tests.IFoo.Bar("""")'
  Expected to find it the number of times specified by the lambda repeat => (repeat > 2) but found it #0 times among the calls:
    1.  'FakeItEasy.Tests.IFoo.Bar()' repeated 2 times
    ...
    3.  'FakeItEasy.Tests.IFoo.Bar([System.Object] test)'
    4.  'FakeItEasy.Tests.IFoo.Bar([System.Object] 4/5/1977 12:00:00 AM, [System.Object] birthday)'
    5.  'FakeItEasy.DynamicProxy.DynamicProxyProxyGenerator+ICanInterceptObjectMembers.ToString()'
    6.  'FakeItEasy.Tests.IFoo.Biz()'

"));
        }
    }
}
