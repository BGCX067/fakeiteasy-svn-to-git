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
    }
}
