using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Legend.Fakes.Api;
using NUnit.Framework.Constraints;
using System.Linq.Expressions;
using System.IO;
using System.Reflection;
using Legend.Fakes.Configuration;

namespace Legend.Fakes.Tests.Api
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

            Assert.That(call.GetDescription(), Is.EqualTo("Legend.Fakes.Tests.IFoo.Bar([System.Object] abc, [System.Object] 123)"));
        }

        [Test]
        public void GetDescription_should_render_NULL_when_argument_is_null()
        {
            var call = CreateFakeCallToFooDotBar(null, 123);

            Assert.That(call.GetDescription(), Is.EqualTo("Legend.Fakes.Tests.IFoo.Bar([System.Object] <NULL>, [System.Object] 123)"));
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
                @"Legend.Fakes.Tests.IFoo.Bar([System.Object] abc, [System.Object] 123)
Legend.Fakes.Tests.IFoo.Bar([System.Object] def, [System.Object] 456)
"));
        }

        [Test]
        public void IsSameMethod_or_derivative_should_be_properly_guarded()
        {
            IsProperlyGuardedConstraint.IsProperlyGuarded(
                () => Helpers.IsSameMethodOrDerivative(typeof(Derived), 
                    typeof(Derived).GetMethod("ToString", new Type[] { }), 
                    typeof(object).GetMethod("ToString", new Type[] { })
                )
            );
        }

        [Test]
        public void IsSameMethodOrDerivative_should_return_true_when_method_is_the_same()
        {
            var method = typeof(IFoo).GetMethod("Bar", new Type[] { });
            var baseMethod = typeof(IFoo).GetMethod("Bar", new Type[] { });

            Assert.That(Helpers.IsSameMethodOrDerivative(typeof(IFoo), method, baseMethod));
        }

        [Test]
        public void IsSameMethodOrDerivative_should_return_false_when_methods_are_unrelated()
        {
            var method = typeof(IFoo).GetMethod("Bar", new Type[] { });
            var baseMethod = typeof(object).GetMethod("ToString", new Type[] { });

            Assert.That(Helpers.IsSameMethodOrDerivative(typeof(IFoo), method, baseMethod), Is.False);
        }

        [Test]
        public void IsSameMethodOrDerivative_should_return_true_when_method_overrides_base_method()
        {
            var method = typeof(Derived).GetMethod("BaseMethod");
            var baseMethod = typeof(Base).GetMethod("BaseMethod");

            Assert.That(Helpers.IsSameMethodOrDerivative(typeof(Derived), method, baseMethod), Is.True);
        }

        [Test]
        public void IsSameMethodOrDerivative_should_return_true_when_method_overrides_method_base_method_is_derived_from()
        {
            var method = typeof(Derived).GetMethod("BaseMethod");
            var baseMethod = typeof(Middle).GetMethod("BaseMethod");

            Assert.That(Helpers.IsSameMethodOrDerivative(typeof(Derived), method, baseMethod), Is.True);
        }

        [Test]
        public void IsSameMethodOrDerivative_should_return_false_when_method_hides_base_method()
        {
            var method = typeof(Derived).GetMethod("BaseNonVirtualMethod");
            var baseMethod = typeof(Base).GetMethod("BaseNonVirtualMethod");

            Assert.That(Helpers.IsSameMethodOrDerivative(typeof(Derived), method, baseMethod), Is.False);
        }

        [Test]
        public void IsSameMethodOrDerivative_should_return_true_when_method_is_interface_method_and_base_method_implementes_that_interface_method()
        {
            var method = typeof(Middle).GetMethod("ToString");
            var baseMethod = typeof(IHideObjectMembers).GetMethod("ToString");

            Assert.That(Helpers.IsSameMethodOrDerivative(typeof(Middle), method, baseMethod), Is.True);
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
