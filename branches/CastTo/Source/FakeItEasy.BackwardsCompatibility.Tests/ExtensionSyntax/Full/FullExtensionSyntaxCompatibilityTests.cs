﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using FullExtensionSyntax = FakeItEasy.ExtensionSyntax.Full.FullExtensionSyntaxCompatibilityExtensions;
using FakeItEasy.Tests;
using FakeItEasy.Assertion;
using FakeItEasy.Configuration;
using FakeItEasy.Api;

namespace FakeItEasy.BackwardsCompatibility.Tests.ExtensionSyntax.Full
{
    [TestFixture]
    public class FullExtensionSyntaxCompatibilityTests
        : ConfigurableServiceLocatorTestBase
    {
        IFakeAssertionsFactory fakeAssertionsFactory;
        IFakeAssertions<IFoo> fakeAssertions;
        IStartConfigurationFactory fakeConfigurationFactory;
        IStartConfiguration<IFoo> fakeConfiguration;

        protected override void OnSetUp()
        {
            this.fakeAssertions = A.Fake<IFakeAssertions<IFoo>>();
            this.fakeAssertionsFactory = A.Fake<IFakeAssertionsFactory>();
            Configure.Fake(this.fakeAssertionsFactory)
                .CallsTo(x => x.CreateAsserter<IFoo>(A<FakeObject>.Ignored))
                .Returns(this.fakeAssertions);

            this.fakeConfiguration = A.Fake<IStartConfiguration<IFoo>>();
            this.fakeConfigurationFactory = A.Fake<IStartConfigurationFactory>();
            Configure.Fake(this.fakeConfigurationFactory)
                .CallsTo(x => x.CreateConfiguration<IFoo>(A<FakeObject>.Ignored))
                .Returns(this.fakeConfiguration);
        }

        [Test]
        public void Assert_should_be_null_guarded()
        {
            NullGuardedConstraint.Assert(() =>
                FullExtensionSyntax.Assert(A.Fake<IFoo>()));
        }

        [Test]
        public void Assert_should_return_fake_assertion_object_from_factory()
        {
            var fake = A.Fake<IFoo>();


            using (Fake.CreateScope())
            {
                this.StubResolve<IFakeAssertionsFactory>(this.fakeAssertionsFactory);
                var assertions = FullExtensionSyntax.Assert(fake);

                Assert.That(assertions, Is.SameAs(this.fakeAssertions));
            }
        }
    }
}
