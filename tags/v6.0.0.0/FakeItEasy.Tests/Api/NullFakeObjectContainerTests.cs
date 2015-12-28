using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using FakeItEasy.Api;

namespace FakeItEasy.Tests.Api
{
    [TestFixture]
    public class NullFakeObjectContainerTests
    {
        [Test]
        public void TryCreateFakeObject_should_return_false()
        {
            var container = new NullFakeObjectContainer();

            object result = null;
            Assert.That(container.TryCreateFakeObject((Type)null, (IEnumerable<object>)null, out result), Is.False);
        }

        [Test]
        public void TryCreateFakeObject_should_set_output_variable_to_null()
        {
            var container = new NullFakeObjectContainer();
            object result = null;

            container.TryCreateFakeObject((Type)null, (IEnumerable<object>)null, out result);

            Assert.That(result, Is.Null);
        }
    }
}
