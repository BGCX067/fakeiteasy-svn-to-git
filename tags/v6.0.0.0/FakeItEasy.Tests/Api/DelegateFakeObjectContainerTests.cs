﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using FakeItEasy.Api;

namespace FakeItEasy.Tests.Api
{
    [TestFixture]
    public class DelegateFakeObjectContainerTests
    {
        [SetUp]
        public void SetUp()
        { 
        
        }

        private DelegateFakeObjectContainer CreateContainer()
        {
            return new DelegateFakeObjectContainer();
        }

        [Test]
        public void TryCreateFakeObject_should_return_object_from_registered_delegate()
        {
            var container = this.CreateContainer();

            var foo = A.Fake<IFoo>();

            container.Register<IFoo>(() => foo);

            object result;
            Assert.That(container.TryCreateFakeObject(typeof(IFoo), null, out result), Is.True);
            Assert.That(result, Is.SameAs(foo));
        }

        [Test]
        public void TryCreateFakeObject_should_return_false_when_no_delegate_is_registered_for_the_requested_type()
        {
            var container = this.CreateContainer();

            object result;
            Assert.That(container.TryCreateFakeObject(typeof(IFoo), null, out result), Is.False);
        }

        [Test]
        public void Register_should_be_able_to_register_the_same_type_twice_to_override_old_registration()
        {
            var container = this.CreateContainer();

            var foo1 = A.Fake<IFoo>();
            var foo2 = A.Fake<IFoo>();

            container.Register<IFoo>(() => foo1);
            container.Register<IFoo>(() => foo2);

            object result;
            container.TryCreateFakeObject(typeof(IFoo), null, out result);

            Assert.That(result, Is.SameAs(foo2));
        }
    }
}
