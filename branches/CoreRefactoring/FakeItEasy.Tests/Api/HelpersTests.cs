using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using FakeItEasy.Api;
using NUnit.Framework.Constraints;
using System.Linq.Expressions;
using System.IO;
using System.Reflection;
using FakeItEasy.Configuration;

namespace FakeItEasy.Tests.Api
{
    [TestFixture]
    public class HelpersTests
    {
        [Test]
        public void GetDescription_should_throw_when_call_is_null()
        {
            Assert.Throws<ArgumentNullException>(() =>
                Helpers.GetDescription((IWritableFakeObjectCall)null));
        }

        [Test]
        public void GetDescription_should_render_method_name_and_empty_arguments_list_when_call_has_no_arguments()
        {
            var call = FakeCall.Create<object>("GetType");

            Assert.That(call.GetDescription(), Is.EqualTo("System.Object.GetType()"));
        }

        [Test]
        public void GetDescription_should_render_method_name_and_all_arguments_when_call_has_arguments()
        {
            var call = CreateFakeCallToFooDotBar("abc", 123);

            Assert.That(call.GetDescription(), Is.EqualTo("FakeItEasy.Tests.IFoo.Bar([System.Object] abc, [System.Object] 123)"));
        }

        [Test]
        public void GetDescription_should_render_NULL_when_argument_is_null()
        {
            var call = CreateFakeCallToFooDotBar(null, 123);

            Assert.That(call.GetDescription(), Is.EqualTo("FakeItEasy.Tests.IFoo.Bar([System.Object] <NULL>, [System.Object] 123)"));
        }

        [Test]
        public void WriteCalls_should_throw_when_writer_is_null()
        {
            Assert.Throws<ArgumentNullException>(() =>
                Helpers.WriteCalls(GetStubCalls(), (TextWriter)null));
        }

        [Test]
        public void WriteCalls_should_throw_when_calls_is_null()
        {
            Assert.Throws<ArgumentNullException>(() =>
                Helpers.WriteCalls((IEnumerable<IFakeObjectCall>)null, new StringWriter()));
        }

        [Test]
        public void WriteCalls_should_write_all_calls_to_writer()
        {
            var calls = new List<IFakeObjectCall> 
            {
                CreateFakeCallToFooDotBar("abc", 123),
                CreateFakeCallToFooDotBar("def", 456)
            };

            var writer = new StringWriter();

            calls.WriteCalls(writer);

            Assert.That(writer.GetStringBuilder().ToString(), Is.EqualTo(
                @"FakeItEasy.Tests.IFoo.Bar([System.Object] abc, [System.Object] 123)
FakeItEasy.Tests.IFoo.Bar([System.Object] def, [System.Object] 456)
"));
        }

        private class Base
        {
            public virtual void BaseMethod()
            { 
            
            }

            public void BaseNonVirtualMethod()
            {
            
            }
        }

        private class Middle
            : Base, IHideObjectMembers
        {
 
        }


        private class Derived
            : Middle
        {
            public override void BaseMethod()
            {
                base.BaseMethod();
            }

            public new void BaseNonVirtualMethod()
            { }
        }

        private static FakeCall CreateFakeCallToFooDotBar(object argument1, object argument2)
        { 
            var call = FakeCall.Create<IFoo>("Bar", new[] { typeof(object), typeof(object) },
                new[] { argument1, argument2 });

            return call;
        }

        private IEnumerable<IFakeObjectCall> GetStubCalls()
        {
            return new List<IFakeObjectCall> 
            {
                CreateFakeCallToFooDotBar("abc", 123),
                CreateFakeCallToFooDotBar("def", 456)
            };
        }
    }
}
