using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Legend.Fakes.Api;

namespace Legend.Fakes.Tests.Api
{
    [TestFixture]
    public class FakeScopeTests
    {
        [Test]
        public void Dispose_sets_outside_scope_as_current_scope()
        {
            var scope = FakeScope.Current;

            using (Fake.CreateScope())
            {
                Assert.That(FakeScope.Current, Is.Not.EqualTo(scope));
            }

            Assert.That(FakeScope.Current, Is.EqualTo(scope));
        }

        [Test]
        public void Disposing_original_scope_does_nothing()
        {
            FakeScope.Current.Dispose();
        }

    }
}
