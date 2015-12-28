using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using FakeItEasy.Tests;
using FakeItEasy.ExtensionSyntax;
using FakeItEasy.VisualBasic;
using System.IO;

namespace FakeItEasy.IntegrationTests
{
    [TestFixture]
    public class WrapperTests
    {
        [Test]
        public void Wrapper_should_only_delegate_non_configured_calls()
        {
            var stream = new MemoryStream();
            var wrapper = A.Fake<Stream>(stream);

            Assert.IsTrue(wrapper.CanRead);

            wrapper.Configure().CallsTo(x => x.CanRead).Returns(false);

            Assert.That(wrapper.CanRead, Is.False);
            Assert.That(stream.CanRead, Is.True);
            Assert.That(CanRead(wrapper), Is.False);
        }

        private bool CanRead(Stream stream)
        {
            return stream.CanRead;
        }
    }
}
