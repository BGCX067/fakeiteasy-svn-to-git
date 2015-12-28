using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Castle.Core.Interceptor;
using FakeItEasy.Api;
using System.Reflection;
using FakeItEasy.DynamicProxy;

namespace FakeItEasy.Tests.DynamicProxy
{
    [TestFixture]
    public class InvocationCallAdapterTests
    {
        [Test]
        public void CallBaseMethod_should_call_Proceed_on_invocation()
        {
            var invokation = A.Fake<IInvocation>();
            Fake.Configure(invokation).CallsTo(x => x.Arguments).Returns(new object[] { });
            Fake.Configure(invokation).CallsTo(x => x.Method).Returns(typeof(IFoo).GetMethod("Bar", new Type[] { }));

            var adapter = new InvocationCallAdapter(invokation);

            adapter.CallBaseMethod();

            Fake.Assert(invokation).WasCalled(x => x.Proceed());
        }

        [Test]
        public void SetArgumentValue_sets_the_argument_value_of_the_invokation()
        {
            var invocation = A.Fake<IInvocation>();
            Fake.Configure(invocation).CallsTo(x => x.Arguments).Returns(new object[] { });
            Fake.Configure(invocation).CallsTo(x => x.Method).Returns(typeof(IFoo).GetMethod("Bar", new Type[] { }));

            var adapter = new InvocationCallAdapter(invocation);

            adapter.SetArgumentValue(0, "test");

            Fake.Assert(invocation)
                .WasCalled(x => x.SetArgumentValue(0, "test"));
        }
    }
}
