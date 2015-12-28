using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace Legend.Fakes.Tests
{
    public abstract class ExceptionTestBase<T> where T : Exception
    {
        [Test]
        public void Constructor_sets_message()
        {
            var ex = CreateException("foo");

            Assert.That(ex.Message, Is.EqualTo("foo"));
        }

        [Test]
        public void Exception_should_be_serializable()
        {
            var ex = CreateException("foo");

            Assert.That(ex, Is.BinarySerializable);
        }

        protected abstract T CreateException(string message);
    }
}
